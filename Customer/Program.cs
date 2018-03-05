using System;
using System.Collections.Generic;
using Core.Application;
using Customer.Client;

namespace Customer.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            string name = "Customer";
            string country = "DK";
            string product = "Product";

            // Check if we are running debug mode without any given arguments, if
            // so ask the user in the console for input, otherwise just use the
            // given arguments.
            if (args.Length == 0)
            {
                Console.WriteLine("Please type in name of customer: ");
                name = Console.ReadLine();

                Console.WriteLine("Please type in country for customer: ");
                country = Console.ReadLine();

                Console.WriteLine("Please type in product to order:");
                product = Console.ReadLine();
            }
            else
            {
                name = args[0];
                country = args[1];
                product = args[2];
            }

            var configuration = new Dictionary<string, string>()
            {
                { "Country", country },
                { "Product", product }
            };

            IApplication application = new CustomerClient(name, configuration);

            application.Start(new string[] { });

            Console.ReadLine();

            application.Stop();
        }
    }
}
