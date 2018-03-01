using System;
using System.Collections.Generic;
using System.Text;
using Core.Application;
using EasyNetQ;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Retailer.Client.Utils;
using Retailer.Messages;

namespace Retailer.Client
{
    public class RetailerClient
        : BaseApplication
    {
        private readonly IBus bus;
        private readonly IMediator mediator;

        public RetailerClient(string name) 
            : base(name)
        {
            this.bus = this.GetService<IBus>();
            this.mediator = this.GetService<IMediator>();
        }

        public override void Start(string[] args)
        {
            this.bus.SubscribeAsync<Retailer.Messages.OrderRequest>("Retailer", this.mediator);
            this.bus.Receive<Warehouse.Messages.OrderResponse>("Retailer", this.mediator);

            this.bus.Subscribe<Retailer.Messages.OrderResponse>(
                "Customer1", message =>
                {
                    Console.WriteLine("Received response.");
                },
                x => x.WithTopic("Customer.Customer1"));

            this.bus.Publish(new Retailer.Messages.OrderRequest(
                Guid.NewGuid().ToString(),
                "DK",
                "Simon",
                "Customer1"));
            
        }

        public override void Stop()
        {
            // Dispose the IBus when application closes.
            this.bus?.Dispose();
        }

        protected override void ConfigureServices(
            IServiceCollection serviceCollection)
        {
            // Add singleton of IBus for EasyNetQ.
            serviceCollection.AddSingleton<IBus>(
                RabbitHutch.CreateBus($"host={this.configuration["RabbitMQ:Host"]}"));

            // Dictionary for contianing broadcast for local order request.
            serviceCollection.AddSingleton<IDictionary<string,
                Retailer.Messages.OrderRequest>>(new Dictionary<string, Retailer.Messages.OrderRequest>());

            // Dictionary for containing broadcast order request to all warehouses.
            serviceCollection
                .AddSingleton<IDictionary<string,
                        Aggregate<Retailer.Messages.OrderRequest, Warehouse.Messages.OrderResponse>>>(new Dictionary<string,
                    Aggregate<Retailer.Messages.OrderRequest, Warehouse.Messages.OrderResponse>>());

            serviceCollection.AddMediatR(typeof(RetailerClient));
        }
    }
}
