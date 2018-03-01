using System;
using System.Collections.Generic;
using System.Text;

namespace Warehouse.Messages
{
    public class OrderRequest
    {
        public OrderRequest(
            string id,
            string countryCode,
            string product,
            string sender,
            string correlationId)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));
            if (string.IsNullOrWhiteSpace(countryCode))
                throw new ArgumentNullException(nameof(countryCode));
            if (string.IsNullOrWhiteSpace(product))
                throw new ArgumentNullException(nameof(product));
            if (string.IsNullOrWhiteSpace(sender))
                throw new ArgumentNullException(nameof(sender));
            if (string.IsNullOrWhiteSpace(correlationId))
                throw new ArgumentNullException(nameof(correlationId));

            this.Id = id;
            this.CountryCode = countryCode;
            this.Product = product;
            this.Sender = sender;
            this.CorrelationId = correlationId;
        }
        public string Id { get; }
        public string CountryCode { get; }
        public string Product { get; }
        public string Sender { get; }
        public string CorrelationId { get; }
    }
}
