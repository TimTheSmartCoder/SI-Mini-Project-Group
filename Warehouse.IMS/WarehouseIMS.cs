using System;
using System.Collections.Generic;
using System.Text;
using Core.Application;
using EasyNetQ;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Warehouse.Messages;

namespace Warehouse.IMS
{
    public class WarehouseIMS
        : BaseApplication
    {
        private readonly IBus bus;
        private readonly IMediator mediator;

        public WarehouseIMS(string name) 
            : base(name)
        {
            this.bus = this.GetService<IBus>();
            this.mediator = this.GetService<IMediator>();
        }

        public override void Start(string[] args)
        {
            var location = args[0];

            this.bus.SubscribeAsync<OrderRequest>(
                this.configuration["Name"], 
                this.mediator,
                c => c.WithTopic("Location.ALL").WithTopic($"Location.{location}"));
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
