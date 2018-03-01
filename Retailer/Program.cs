using System;
using Core.Application;

namespace Retailer.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            IApplication application = new RetailerClient("Retailer");

            application.Start(args);

            Console.ReadLine();

            application.Stop();
        }
    }
}
