using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;
using MediatR;
using Microsoft.Extensions.Logging;
using Warehouse.IMS.CommandHandlers;
using Warehouse.Messages;

namespace Warehouse.IMS.MessageHandlers
{
    public class OrderRequestMessageHandler
        : RequestMessageHandler<OrderRequest>
    {
        private readonly IMediator mediator;
        private readonly ILogger<OrderRequestMessageHandler> logger;

        public OrderRequestMessageHandler(
            IMediator mediator,
            ILogger<OrderRequestMessageHandler> logger)
        {
            if(mediator == null)
                throw new ArgumentNullException(nameof(mediator));

            this.mediator = mediator;
            this.logger = logger;
        }
        public override async Task Run(OrderRequest message)
        {
            this.logger.LogInformation("Recevied OrderRequest from Retailer.");

            var command = new SendOrderResponse(message);

            await this.mediator.Send(command);
        }
    }
}
