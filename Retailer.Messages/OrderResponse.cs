using System;
using System.Collections.Generic;
using System.Text;

namespace Retailer.Messages
{
    public class OrderResponse
    {
        public OrderResponse(
            string id, 
            string sender, 
            string product, 
            DateTime? delivery,
            double shippingCharge,
            int stock,
            string shippingFrom)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));
            if (string.IsNullOrWhiteSpace(sender))
                throw new ArgumentNullException(nameof(sender));
            if (string.IsNullOrWhiteSpace(product))
                throw new ArgumentNullException(nameof(product));

            this.Id = id;
            this.Sender = sender;
            this.Product = product;
            this.Delivery = delivery;
            this.ShippingCharge = shippingCharge;
            this.Stock = stock;
            this.ShippingFrom = shippingFrom;
        }

        public string Id { get; }
        public string Sender { get; }
        public string Product { get; }
        public DateTime? Delivery { get; }
        public double ShippingCharge { get; }
        public int Stock { get; }
        public string ShippingFrom { get; }
    }
}
