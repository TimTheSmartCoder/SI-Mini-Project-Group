using System;
using System.Collections.Generic;
using System.Text;

namespace Customer.Client.Entities
{
    public class Order
    {
        public Order(
            string country,
            string product)
        {
            if (string.IsNullOrWhiteSpace(country))
                throw new ArgumentNullException(nameof(country));
            if (string.IsNullOrWhiteSpace(product))
                throw new ArgumentNullException(nameof(product));

            this.Country = country;
            this.Product = product;
        }

        public string Country { get; }

        public string Product { get; }
    }

    public class OrderConfirmation
    {
        public OrderConfirmation(
            string country,
            DateTime delivery,
            string product,
            double shippingCharge,
            int stock,
            string shippingFrom)
        {
            if (string.IsNullOrWhiteSpace(country))
                throw new ArgumentNullException(nameof(country));
            if (string.IsNullOrWhiteSpace(product))
                throw new ArgumentNullException(nameof(product));

            this.Country = country;
            this.Delivery = delivery;
            this.Product = product;
            this.ShippingCharge = shippingCharge;
            this.Stock = stock;
            this.ShippingFrom = shippingFrom;
        }

        public string Country { get; }

        public DateTime Delivery { get;  }

        public string Product { get; }

        public double ShippingCharge { get; }

        public int Stock { get; }

        public string ShippingFrom { get; }
        
    }
}
