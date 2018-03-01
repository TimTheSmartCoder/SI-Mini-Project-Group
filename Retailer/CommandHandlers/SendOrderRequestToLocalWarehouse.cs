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
    public class SendOrderRequestToLocalWarehouse 
        : IRequest
    {
        public SendOrderRequestToLocalWarehouse(OrderRequest order)
        {
            if(order == null)
                throw new ArgumentNullException(nameof(order));
            
            this.Order = order;
        }

        public OrderRequest Order;        
    }

    public class SendOrderRequestToLocalWarehouseHandler
        : IRequestHandler<SendOrderRequestToLocalWarehouse>
    {
        private readonly IBus bus;
        private readonly ILogger<SendOrderRequestToLocalWarehouseHandler> logger;
        private readonly IDictionary<string, Retailer.Messages.OrderRequest> localOrderRequests;
        private readonly IConfiguration configuration;

        public SendOrderRequestToLocalWarehouseHandler(
            IBus bus,
            ILogger<SendOrderRequestToLocalWarehouseHandler> logger,
            IDictionary<string, Retailer.Messages.OrderRequest> localOrderRequests,
            IConfiguration configuration)
        {
            if (bus == null)
                throw new ArgumentNullException(nameof(bus));

            this.bus = bus;
            this.logger = logger;
            this.localOrderRequests = localOrderRequests;
            this.configuration = configuration;
        }

        public Task Handle(
            SendOrderRequestToLocalWarehouse message,
            CancellationToken cancellationToken)
        {
            var sender = this.configuration["Name"];

            var request = new Warehouse.Messages.OrderRequest(
                Guid.NewGuid().ToString(),
                message.Order.CountryCode,
                message.Order.Product,
                sender,
                Guid.NewGuid().ToString());

            this.localOrderRequests.Add(request.CorrelationId, message.Order);

            this.bus.Publish(request, $"Location.{message.Order.CountryCode}");

            this.logger.LogInformation("Sent order request to local warehouse.");

            return Task.FromResult(true);
        }
    }
}
