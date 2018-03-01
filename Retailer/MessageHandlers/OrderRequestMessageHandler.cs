using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;
using MediatR;
using Microsoft.Extensions.Logging;
using Retailer.Client.CommandHandlers;
using Retailer.Messages;

namespace Retailer.Client.MessageHandlers
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
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));

            this.mediator = mediator;
            this.logger = logger;
        }

        public override async Task Run(OrderRequest message)
        {
            this.logger.LogInformation("Recevied OrderRequest from Customer.");

            // Create command for sending request to local warehouse.
            var command = new SendOrderRequestToLocalWarehouse(message);

            try
            {
                // Send command to mediato, which sends the command to the
                // correct handler.
                await this.mediator.Send(command);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
