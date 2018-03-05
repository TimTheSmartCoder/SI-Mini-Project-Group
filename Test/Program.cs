using System;
using System.Diagnostics;
using System.IO;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Process process = Process.Start(new ProcessStartInfo()
                {
                    FileName = "dotnet",
                    WorkingDirectory = Path.Combine(Environment.CurrentDirectory, "bin/Debug/netcoreapp2.0"),
                    Arguments = "Customer.Client.dll --no-build -- Customer1 DK product",
                    
                });
                Console.ReadLine();
                process.WaitForExit();
                
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
            Console.ReadLine();
        }
    }
}
