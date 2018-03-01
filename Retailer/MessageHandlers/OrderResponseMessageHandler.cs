using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Retailer.Client.Utils;
using Warehouse.Messages;

namespace Retailer.Client.MessageHandlers
{
    public class OrderResponseMessageHandler 
        : RequestMessageHandler<OrderResponse>
    {
        private readonly IMediator mediator;
        private readonly IDictionary<string, Retailer.Messages.OrderRequest> localOrderRequests;
        private IDictionary<string, Aggregate<Retailer.Messages.OrderRequest, Warehouse.Messages.OrderResponse>>
            globalOrderRequests;

        private readonly IBus bus;
        private readonly IConfiguration configuration;
        private readonly ILogger<OrderResponseMessageHandler> logger;

        public OrderResponseMessageHandler(
            IDictionary<string, Retailer.Messages.OrderRequest> localOrderRequests,
            IDictionary<string, Aggregate<Retailer.Messages.OrderRequest, Warehouse.Messages.OrderResponse>> globalOrderRequests,
            IMediator mediator,
            IBus bus,
            IConfiguration configuration,
            ILogger<OrderResponseMessageHandler> logger)
        {
            this.localOrderRequests = localOrderRequests;
            this.mediator = mediator;
            this.bus = bus;
            this.globalOrderRequests = globalOrderRequests;
            this.configuration = configuration;
            this.logger = logger;
        }

        private void SendResponse(
            Retailer.Messages.OrderRequest customerOrderRequest,
            Warehouse.Messages.OrderResponse warehouseOrderResponse,
            Retailer.Messages.OrderResponse.OrderResponseStatus status)
        {
            var sender = this.configuration["Name"];

            DateTime? delivery = null;

            if (warehouseOrderResponse != null)
                delivery = warehouseOrderResponse.Delivery;

            // Response to the customer for the order request.
            var customerResponse = new Retailer.Messages.OrderResponse(
                Guid.NewGuid().ToString(),
                sender,
                customerOrderRequest.Product,
                delivery,
                status);

            this.bus.Publish(customerResponse, $"Customer.{customerOrderRequest.Sender}");
        }

        private void SendToAllWarehouses(
            Retailer.Messages.OrderRequest customerOrderRequest)
        {
            var sender = this.configuration["Name"];
            var correlationId = Guid.NewGuid().ToString();

            var warehouseORderRequest = new Warehouse.Messages.OrderRequest(
                Guid.NewGuid().ToString(), 
                customerOrderRequest.CountryCode, 
                customerOrderRequest.Product, 
                sender, 
                correlationId);

            var aggreateRequest = new Aggregate<Retailer.Messages.OrderRequest, Warehouse.Messages.OrderResponse>(
                correlationId, 2, customerOrderRequest);

            this.globalOrderRequests.Add(correlationId, aggreateRequest);

            this.bus.Publish(warehouseORderRequest, "Location.ALL");
        }

        public override Task Run(OrderResponse response)
        {
            // Check if response is for a local warehouse request.
            if (this.localOrderRequests.ContainsKey(response.CorrelationId))
            {
                this.logger.LogInformation("Received response from Warehouse.");

                var localWarehouseResponse = response;

                // Get Customer order request with the same correlation id
                // as the given response from Warehouse.
                var customerOrderRequest = this.localOrderRequests[response.CorrelationId];

                // Remove the Customer order request with the
                // same correlation id, because we have know received and
                // answer for the local warehouse request.
                this.localOrderRequests.Remove(localWarehouseResponse.CorrelationId);

                if (response.Status == OrderResponseStatus.Succes)
                {
                    // Recevied and successfull response from the warehouse
                    // so we can respond to the customer.
                    SendResponse(customerOrderRequest, localWarehouseResponse, Retailer.Messages.OrderResponse.OrderResponseStatus.Success);
                }
                else
                {
                    // The response from the local warehouse were not successfull, either a failure
                    // our out of stock, so we contact all warehouses instead.
                    SendToAllWarehouses(customerOrderRequest);
                }

            }
            else if (this.globalOrderRequests.ContainsKey(response.CorrelationId))
            {
                this.logger.LogInformation("Received aggregate response from Warehouse.");

                // Response from warehouse for an request to all warehouses.
                var globalWarehouseResponse = response;

                // Get the aggregate containing response from the all request to warehouses.
                var requestAggregate = this.globalOrderRequests[globalWarehouseResponse.CorrelationId];

                // Add the response from the warehouse to the list of responses from other warehouses.
                requestAggregate.Add(globalWarehouseResponse);

                // Check if we have received the number of responses from warehouses that the
                // aggregate are interrested in, so we can send and response the customer.
                if (requestAggregate.IsComplete())
                {
                    // Get the best response from the warehouse, we have gotton.
                    var bestResponseForCustomer =
                        requestAggregate.GetResult(x => x.Status == OrderResponseStatus.Succes);

                    if (bestResponseForCustomer == null)
                    {
                        // No response were successfull, so we notify the user  that
                        // the product they requested is out of stock.
                        SendResponse(
                            requestAggregate.Request, 
                            null, 
                            Retailer.Messages.OrderResponse.OrderResponseStatus.Failure);
                    }
                    else
                    {
                        // Response were successfull and we can notify the user
                        // about the response.
                        SendResponse(
                            requestAggregate.Request, 
                            bestResponseForCustomer,
                            Retailer.Messages.OrderResponse.OrderResponseStatus.Failure);
                    }

                    // Remove the aggregate because it is complete we have received
                    // the number of responses we expect.
                    this.globalOrderRequests.Remove(requestAggregate.CorrelationId);
                }
            }
            else
            {
                // We ignore the response messages received, the messages is probally and response
                // to an order request to all warehouse, but we have gotten the number of request
                // we are interrested in, so we can safely ignore the response.
            }

            return Task.FromResult(true);
        }
    }
}
