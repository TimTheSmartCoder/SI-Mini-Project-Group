using System;
using System.Collections.Generic;
using System.Text;
using Core.Application;
using EasyNetQ;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Warehouse.Messages;

namespace Warehouse.IMS
{
    public class WarehouseIMS
        : BaseApplication
    {
        private readonly IBus bus;
        private readonly IMediator mediator;
        private readonly ILogger<WarehouseIMS> logger;

        public WarehouseIMS(string name, Dictionary<string, string> configuration) 
            : base(name, configuration)
        {
            this.bus = this.GetService<IBus>();
            this.mediator = this.GetService<IMediator>();
            this.logger = this.GetService<ILogger<WarehouseIMS>>();
        }

        public override void Start(string[] args)
        {
            var location = this.configuration["Country"];

            var name = this.configuration["Name"];

            this.bus.SubscribeAsync<OrderRequest>(
                name, 
                this.mediator,
                c => c.WithTopic("Location.ALL").WithTopic($"Location.{location}"));

           this.logger.LogInformation($"Listening for incoming order requests({name}):");
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

            serviceCollection.AddMediatR(typeof(WarehouseIMS));
        }
    }
}
