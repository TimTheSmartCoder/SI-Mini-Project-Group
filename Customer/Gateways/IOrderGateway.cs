using System;
using System.Collections.Generic;
using System.Text;
using Customer.Client.Entities;

namespace Customer.Client.Gateways
{
    public interface IOrderGateway
    {
        /// <summary>
        /// Places an order with Retailer, will return null if
        /// error happen, or retailer do not respond in a time
        /// perioed.
        /// </summary>
        /// <param name="order">Order to send to retailer.</param>
        /// <returns></returns>
        OrderConfirmation PlaceOrder(Order order);
    }
}
