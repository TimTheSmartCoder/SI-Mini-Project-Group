using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Core.Application
{
    public abstract class BaseApplication
        : IApplication
    {
        protected readonly IConfiguration configuration;
        private readonly IServiceProvider serviceProvider;

        protected BaseApplication(string name, Dictionary<string, string> configuration)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            // Create default configuration object.
            this.configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>()
                {
                    { "Name", name }
                })
                .AddInMemoryCollection(configuration)
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            
            IServiceCollection serviceCollection = new ServiceCollection();

            // Add configuration to the dependency injection.
            serviceCollection.AddSingleton<IConfiguration>(this.configuration);

            // Add logging factory to the service collection
            // for dependency injection of logger services.
            serviceCollection.AddSingleton(new LoggerFactory()
                .AddConsole()
                .AddDebug(LogLevel.Information));

            // Call ConfigureServices on child to add possible services
            // to the dependency injection collection.
            this.ConfigureServices(serviceCollection);

            // Build the dependency injection service provider from
            // collection of services.
            this.serviceProvider = serviceCollection
                .AddLogging()
                .BuildServiceProvider();
        }

        public abstract void Start(string[] args);

        public abstract void Stop();

        protected abstract void ConfigureServices(
            IServiceCollection serviceCollection);

        /// <summary>
        /// Gets the service which has the given interface.
        /// </summary>
        /// <typeparam name="TService">Interface of services to get.</typeparam>
        /// <returns></returns>
        protected TService GetService<TService>()
        {
            return this.serviceProvider.GetRequiredService<TService>();
        }
    }
}
