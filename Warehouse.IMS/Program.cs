using System;
using System.Collections.Generic;
using Core.Application;

namespace Warehouse.IMS
{
    class Program
    {
        static void Main(string[] args)
        {
            string country = "DK";
            string inStock = "false";

            if (args.Length == 0)
            {
                Console.WriteLine("Please type in country code for warehouse: ");
                country = Console.ReadLine();

                Console.WriteLine("Should response with the product, be in stock? (true or false):");
                inStock = Console.ReadLine();
            }
            else
            {
                country = args[0];
            }

            var configuration = new Dictionary<string, string>()
            {
                { "Country", country },
                { "InStock", inStock }
            };

            IApplication application = new WarehouseIMS($"Warehouse.{country}", configuration);

            application.Start(new string[] { });

            Console.ReadLine();

            application.Stop();
        }
    }
}
