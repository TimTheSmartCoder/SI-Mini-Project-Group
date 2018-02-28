using System;
using Core.Application;

namespace Warehouse.IMS
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Plase type in country code: ");

            string countryCode = Console.ReadLine();

            IApplication application = new WarehouseIMS($"Warehouse.{countryCode}");

            application.Start(args);

            Console.ReadLine();

            application.Stop();
        }
    }
}
