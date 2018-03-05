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
    public class SendCustomerFailureResponse 
        : IRequest
    {
        public SendCustomerFailureResponse(
            Retailer.Messages.OrderRequest customerOrderRequest)
        {
            if (customerOrderRequest == null)
                throw new ArgumentNullException(nameof(customerOrderRequest));

            this.CustomerOrderRequest = customerOrderRequest;
        }

        public Retailer.Messages.OrderRequest CustomerOrderRequest { get; }
    }

    public class SendCustomerFailureResponseHandler
        : IRequestHandler<SendCustomerFailureResponse>
    {
        private readonly IBus bus;
        private readonly ILogger<SendCustomerFailureResponseHandler> logger;
        private readonly IConfiguration configuration;

        public SendCustomerFailureResponseHandler(
            IBus bus,
            ILogger<SendCustomerFailureResponseHandler> logger,
            IConfiguration configuration)
        {
            this.bus = bus;
            this.logger = logger;
            this.configuration = configuration;
        }

        public Task Handle(SendCustomerFailureResponse message, CancellationToken cancellationToken)
        {
            var sender = this.configuration["Name"];

            // Response to the customer for the order request.
            var customerResponse = new Retailer.Messages.OrderResponse(
                Guid.NewGuid().ToString(),
                sender,
                message.CustomerOrderRequest.Product,
                null,
                0.0,
                0,
                "");

            this.bus.Publish(customerResponse, $"Customer.{message.CustomerOrderRequest.Sender}");

            this.logger.LogInformation("Sent failure order response to customer.");

            return Task.FromResult(true);
        }
    }
}
