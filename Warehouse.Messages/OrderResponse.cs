using System;
using System.Collections.Generic;
using System.Text;

namespace Warehouse.Messages
{
    public class OrderResponse
    {
        public OrderResponse(
            string id,
            string sender,
            string correlationId,
            DateTime? delivery,
            double shippingCharge,
            int stock,
            string shippingFrom)
        {
            if(string.IsNullOrWhiteSpace(id))
                throw  new ArgumentNullException(nameof(id));
            if (string.IsNullOrWhiteSpace(sender))
                throw new ArgumentNullException(nameof(sender));
            if (string.IsNullOrWhiteSpace(correlationId))
                throw new ArgumentNullException(nameof(correlationId));

            this.Id = id;
            this.Sender = sender;
            this.CorrelationId = correlationId;
            this.Delivery = delivery;
            this.ShippingCharge = shippingCharge;
            this.Stock = stock;
            this.ShippingFrom = shippingFrom;
        }

        public string Id { get; }
        public string Sender { get; }
        public string CorrelationId { get; }
        public DateTime? Delivery { get; }
        public double ShippingCharge { get; }
        public int Stock { get; }
        public string ShippingFrom { get; }
    }

    public enum OrderResponseStatus
    {
        Succes,
        OutOfStock,
        Failure
    }
}
