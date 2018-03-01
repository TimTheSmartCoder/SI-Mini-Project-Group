using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Retailer.Messages;

namespace Retailer.Client.CommandHandlers
{
    public class SendCustomerSuccesResponse : IRequest
    {
        public SendCustomerSuccesResponse(
            Retailer.Messages.OrderRequest customerOrderRequest,
            Warehouse.Messages.OrderResponse warehouseOrderResponse)
        {
            if (customerOrderRequest == null)
                throw new ArgumentNullException(nameof(customerOrderRequest));
            if(warehouseOrderResponse == null)
                throw  new ArgumentNullException(nameof(warehouseOrderResponse));

            this.CustomerOrderRequest = customerOrderRequest;
            this.WarehouseOrderResponse = warehouseOrderResponse;
        }

        public Retailer.Messages.OrderRequest CustomerOrderRequest { get; }
        public Warehouse.Messages.OrderResponse WarehouseOrderResponse { get; }
    }

    public class SendCustomerSuccesResponseHandler : IRequestHandler<SendCustomerSuccesResponse>
    {
        private readonly IBus bus;
        private readonly ILogger<SendCustomerSuccesResponseHandler> logger;        
        private readonly IConfiguration configuration;

        public SendCustomerSuccesResponseHandler(
            IBus bus, 
            ILogger<SendCustomerSuccesResponseHandler> logger, 
            IConfiguration configuration)
        {
            this.bus = bus;
            this.logger = logger;
            this.configuration = configuration;
        }

        public Task Handle(SendCustomerSuccesResponse message, CancellationToken cancellationToken)
        {
            var sender = this.configuration["Name"];
            
            // Response to the customer for the order request.
            var customerResponse = new Retailer.Messages.OrderResponse(
                Guid.NewGuid().ToString(),
                sender,
                message.CustomerOrderRequest.Product,
                message.WarehouseOrderResponse.Delivery,
                OrderResponse.OrderResponseStatus.Success);

            this.bus.Publish(customerResponse, $"Customer.{message.CustomerOrderRequest.Sender}");

            return Task.FromResult(true);
        }
    }
}
