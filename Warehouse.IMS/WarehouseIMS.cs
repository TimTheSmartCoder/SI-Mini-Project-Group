using System;
using System.Collections.Generic;
using System.Text;
using Core.Application;
using EasyNetQ;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Warehouse.IMS
{
    public class WarehouseIMS
        : BaseApplication
    {
        private readonly IBus bus;

        public WarehouseIMS(string name) 
            : base(name)
        {
            this.bus = this.GetService<IBus>();
        }

        public override void Start(string[] args)
        { }

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
