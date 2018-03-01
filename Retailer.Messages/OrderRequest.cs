using System;
using System.Collections.Generic;
using System.Text;

namespace Retailer.Messages
{
    public class OrderRequest
    {
        public OrderRequest(
            string id, 
            string countryCode, 
            string product, 
            string sender)
        {          
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));
            if (string.IsNullOrWhiteSpace(countryCode))
                throw new ArgumentNullException(nameof(countryCode));
            if (string.IsNullOrWhiteSpace(product))
                throw new ArgumentNullException(nameof(product));
            if (string.IsNullOrWhiteSpace(sender))
                throw new ArgumentNullException(nameof(sender));           

            this.Id = id;
            this.CountryCode = countryCode;
            this.Product = product;
            this.Sender = sender;
        }
        public string Id { get; }
        public string CountryCode { get; }
        public string Product { get; }
        public string Sender { get; }
    }
}
