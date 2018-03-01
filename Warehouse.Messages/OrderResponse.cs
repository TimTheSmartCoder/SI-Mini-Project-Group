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
            OrderResponseStatus status,
            DateTime delivery)
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
            this.Status = status;
            this.Delivery = delivery;

        }

        public string Id { get; }
        public string Sender { get; }
        public string CorrelationId { get; }
        public OrderResponseStatus Status { get; }
        public DateTime Delivery { get; }
    }

    public enum OrderResponseStatus
    {
        Succes,
        OutOfStock,
        Failure
    }
}
