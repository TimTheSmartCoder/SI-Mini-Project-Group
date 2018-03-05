using System;
using System.Collections.Generic;
using Core.Application;

namespace Retailer.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            IApplication application = new RetailerClient("Retailer", new Dictionary<string, string>());

            application.Start(args);

            Console.ReadLine();

            application.Stop();
        }
    }
}
