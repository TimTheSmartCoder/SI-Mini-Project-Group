using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Retailer.Client.Utils;

namespace Retailer.Client.CommandHandlers
{
    public class SendOrderRequestToAllWarehouses : IRequest
    {
        public SendOrderRequestToAllWarehouses(Retailer.Messages.OrderRequest customerOrderRequest)
        {
            if(customerOrderRequest == null)
                throw new ArgumentNullException(nameof(customerOrderRequest));

            this.CustomerOrderRequest = customerOrderRequest;
        }

        public Retailer.Messages.OrderRequest CustomerOrderRequest { get; }
    }

    public class SendOrderRequestToAllWarehousesHandler : IRequestHandler<SendOrderRequestToAllWarehouses>
    {
        private readonly IBus bus;
        private readonly ILogger<SendCustomerSuccesResponseHandler> logger;
        private readonly IConfiguration configuration;
        private IDictionary<string, Aggregate<Retailer.Messages.OrderRequest, Warehouse.Messages.OrderResponse>>
            globalOrderRequests;

        public SendOrderRequestToAllWarehousesHandler(
            IBus bus,
            ILogger<SendCustomerSuccesResponseHandler> logger,
            IConfiguration configuration,
            IDictionary<string, Aggregate<Retailer.Messages.OrderRequest, Warehouse.Messages.OrderResponse>>
            globalOrderRequests
            )
        {
            this.bus = bus;
            this.logger = logger;
            this.configuration = configuration;
            this.globalOrderRequests = globalOrderRequests;
        }

        public Task Handle(
            SendOrderRequestToAllWarehouses message, 
            CancellationToken cancellationToken)
        {
            var sender = this.configuration["Name"];
            var correlationId = Guid.NewGuid().ToString();

            var warehouseORderRequest = new Warehouse.Messages.OrderRequest(
                Guid.NewGuid().ToString(),
                message.CustomerOrderRequest.CountryCode,
                message.CustomerOrderRequest.Product,
                sender,
                correlationId);

            var aggreateRequest = new Aggregate<Retailer.Messages.OrderRequest, Warehouse.Messages.OrderResponse>(
                correlationId, 2, message.CustomerOrderRequest);

            this.globalOrderRequests.Add(correlationId, aggreateRequest);

            this.bus.Publish(warehouseORderRequest, "Location.ALL");

            return Task.FromResult(true);
        }
    }
}
