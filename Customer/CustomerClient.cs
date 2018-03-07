using System;
using System.Collections.Generic;
using System.Text;
using Core.Application;
using Customer.Client.Entities;
using Customer.Client.Gateways;
using EasyNetQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Customer.Client
{
    public class CustomerClient
        : BaseApplication
    {
        private readonly IBus bus;
        private readonly IOrderGateway orderGateway;

        public CustomerClient(string name, Dictionary<string, string> configuration) 
            : base(name, configuration)
        {
            this.bus = this.GetService<IBus>();
            this.orderGateway = this.GetService<IOrderGateway>();
        }

        public override void Start(string[] args)
        {
            // Get the country from the first argument.
            var country = this.configuration["Country"];
            var product = this.configuration["Product"];

            try
            {
                var confirmation = this.orderGateway.PlaceOrder(
                    new Order(country, product));

                Console.WriteLine("Product has been succesfully ordered.");
                Console.WriteLine("Order information:");
                Console.WriteLine($"Product: {confirmation.Product}");
                Console.WriteLine($"Delivery date: {confirmation.Delivery}");
                Console.WriteLine($"Shipping Charge: {confirmation.ShippingCharge}");
                Console.WriteLine($"Shipping From(Country): {confirmation.ShippingFrom}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }

            Console.WriteLine("Press Enter to exit application");
        }

        public override void Stop()
        {
            // Dispose the IBus when application closes.
            this.bus?.Dispose();
        }

        protected override void ConfigureServices(IServiceCollection serviceCollection)
        {
            // Add singleton of IBus for EasyNetQ.
            serviceCollection.AddSingleton<IBus>(
                RabbitHutch.CreateBus($"host={this.configuration["RabbitMQ:Host"]}"));
            
            serviceCollection.AddSingleton<IOrderGateway, OrderGateway>();
        }
    }
}
