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
            OrderResponseStatus status)
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
            this.Status = status;
        }

        public string Id { get; }
        public string Sender { get; }
        public string Product { get; }
        public DateTime? Delivery { get; }
        public OrderResponseStatus Status { get; }

        public enum OrderResponseStatus
        {
            Success,
            OutOfStock,
            Failure
        }
    }
}
