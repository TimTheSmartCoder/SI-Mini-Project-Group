using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Customer.Client.Entities;
using EasyNetQ;
using Microsoft.Extensions.Configuration;
using Retailer.Messages;

namespace Customer.Client.Gateways
{
    public class OrderGateway
        : IOrderGateway
    {
        private readonly IBus bus;
        private readonly IConfiguration configuration;

        private OrderResponse orderResponse;

        public OrderGateway(IBus bus, IConfiguration configuration)
        {
            if (bus == null)
                throw new ArgumentNullException(nameof(bus));
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            this.bus = bus;
            this.configuration = configuration;

            // Subscribe for listen for reply messages.
            this.Subscribe();
        }

        public OrderConfirmation PlaceOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            // Get data for order request.
            var id = Guid.NewGuid().ToString();
            var country = order.Country;
            var product = order.Product;
            var sender = this.configuration["Name"];

            // Prepare order request.
            var orderRequest = new OrderRequest(
                id,
                country,
                product,
                sender);

            // Get the queue which belongs to the retailer.
            var retailer = this.configuration["Retailer"];

            // Send order request to retailer.
            this.bus.Send(retailer, orderRequest);

            var hasRetailerReplied = false;

            lock (this)
            {
                hasRetailerReplied = Monitor.Wait(this, 2000);
            }

            // Retailer has not replied in time, so we
            // return null to indicate failure.
            if (!hasRetailerReplied)
                throw new Exception("Timeout on request, please try again.");

            if (this.orderResponse.Stock == 0)
                throw new Exception("Product is out of stock.");

            var result = new OrderConfirmation(
                order.Country,
                this.orderResponse.Delivery ?? DateTime.Now,
                this.orderResponse.Product,
                this.orderResponse.ShippingCharge,
                this.orderResponse.Stock,
                this.orderResponse.ShippingFrom);

            return result;
        }

        private void Subscribe()
        {
            // Get the name of the customer.
            var name = this.configuration["Name"];

            // Create subscription from name.
            var subscriptionId = $"Customer.{name}";

            this.bus.Subscribe<OrderResponse>(subscriptionId, (orderResponse) =>
            {
                // Lock the current thread, to wake the thread up from sleep.

                lock (this)
                {
                    // Save the order response from the retailer.
                    this.orderResponse = orderResponse;
                    
                    // Wake up thread.
                    Monitor.Pulse(this);
                }
            });
        }
    }
}
