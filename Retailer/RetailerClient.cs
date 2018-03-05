using System;
using System.Collections.Generic;
using System.Text;
using Core.Application;
using EasyNetQ;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Retailer.Client.Utils;
using Retailer.Messages;

namespace Retailer.Client
{
    public class RetailerClient
        : BaseApplication
    {
        private readonly IBus bus;
        private readonly IMediator mediator;
        private readonly ILogger<RetailerClient> logger;

        public RetailerClient(string name, Dictionary<string, string> configuration) 
            : base(name, configuration)
        {
            this.bus = this.GetService<IBus>();
            this.mediator = this.GetService<IMediator>();
            this.logger = this.GetService<ILogger<RetailerClient>>();
        }

        public override void Start(string[] args)
        {
            this.logger.LogInformation("Starting to listen for incoming messages.");

            var subscription = this.bus.Receive("Retailer", 
                x => x
                    .Add<Retailer.Messages.OrderRequest>(this.mediator)
                    .Add< Warehouse.Messages.OrderResponse>(this.mediator));

            this.logger.LogInformation("Listening for messages.");
            
            Console.ReadLine();

            this.logger.LogInformation("Stopping listening for messages.");
            
            subscription.Dispose();

            this.logger.LogInformation("Stopped listening for messages.");
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
