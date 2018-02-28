using System;
using System.Collections.Generic;
using System.Text;
using Core.Application;
using EasyNetQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Customer.Client
{
    public class CustomerClient
        : BaseApplication
    {
        private readonly IBus bus;

        public CustomerClient(string name) 
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
        }
    }
}
