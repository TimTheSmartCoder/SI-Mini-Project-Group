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
            // Warehouse name.
            var warehouseName = configuration["Name"];

            // Country of the warehouse.
            var warehouseCountry = this.configuration["Country"];

            bool inStock = bool.Parse(this.configuration["InStock"]);

            var id = Guid.NewGuid().ToString();
            var sender = warehouseName;
            var correlationId = message.Order.CorrelationId;
            DateTime? delivery = null;
            double shippingCharge = 0.0;
            var stock = 0;
            var shippingFrom = warehouseCountry;
            
            // Add shipping charge and delivery information to response,
            // out from warehouse location, if the warehouse and customer
            // is in the same country delivery is faster and cheaper.
            if (message.Order.CountryCode.Equals(warehouseCountry))
            {
                shippingCharge = 2.0;
                delivery = DateTime.Now.AddDays(2);
            }
            else
            {
                shippingCharge = 10.0;
                delivery = DateTime.Now.AddDays(10);
            }

            // Set the stock count to an random number, if the
            // product is in stock.
            if (inStock)
                stock = new Random().Next(1, 100);

            var orderResponse = new OrderResponse(
                id,
                sender,
                correlationId,
                delivery,
                shippingCharge,
                stock,
                shippingFrom);

            this.bus.Send(message.Order.Sender, orderResponse);

            this.logger.LogInformation("Sent order response to Retailer.");

            return Task.FromResult(true);
        }
        
    }
}
