using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Warehouse.Messages;

namespace Warehouse.IMS.CommandHandlers
{
    public class SendOrderResponse
        : IRequest
    {
        public SendOrderResponse(OrderRequest message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            this.Order = message; 
        }

        public OrderRequest Order { get;  }
    }

    public class SendOrderResponseHandler 
        : IRequestHandler<SendOrderResponse>
    {
        private readonly IBus bus;
        private readonly IConfiguration configuration;
        private readonly ILogger<SendOrderResponseHandler> logger;

        public SendOrderResponseHandler(
            IBus bus, 
            IConfiguration configuration,
            ILogger<SendOrderResponseHandler> logger)
        {
            if(bus == null)
                throw new ArgumentNullException(nameof(bus));
            if(configuration == null)
                throw  new ArgumentNullException(nameof(configuration));

            this.bus = bus;
            this.configuration = configuration;
            this.logger = logger;
        }

        public Task Handle(
            SendOrderResponse message, 
            CancellationToken cancellationToken)
        {
            var sender = configuration["Name"];

            var response = new OrderResponse(
                Guid.NewGuid().ToString(), 
                sender, message.Order.CorrelationId, 
                OrderResponseStatus.OutOfStock, 
                DateTime.Now);   


            this.bus.Send(message.Order.Sender, response);

            this.logger.LogInformation("Sent order response to Retailer.");

            return Task.FromResult(true);
        }
    }
}
