using System;
using Core.Application;
using Customer.Client;

namespace Customer.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            IApplication application = new CustomerClient("Customer");

            application.Start(args);

            Console.ReadLine();

            application.Stop();
        }
    }
}
