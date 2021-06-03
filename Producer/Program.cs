using System;
using System.IO;
using System.Threading;

using Lab.Shared.Configuration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Producer.Interfaces;

namespace Producer
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureHostConfiguration((configuration) =>
                {
                    // Add config file support.
                    // By default, appsettings.json should be located in the same folder as Program.cs
                    configuration.SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .Build();
                })
                // Dependency injection configuring
                .ConfigureServices((hostingContext, services) =>
                {
                    // Adding producer as a singleton service
                    services.AddSingleton<IProducer, Producer>();
                    
                    // Configuring producer`s options (converting appsettings.json to C# class object)
                    services.Configure<ProducerOptions>(hostingContext.Configuration.GetSection("Producer"));
                })
                .Build();

            // Creating a Producer instance
            var logsProducer = ActivatorUtilities.CreateInstance<Producer>(host.Services);
            
            // Creating a separated thread for producer
            var producerThread = new Thread(() => logsProducer.ExecuteSending())
            {
                Name = "Producer thread",
                IsBackground = false,
                Priority = ThreadPriority.Normal
            };
            producerThread.Start();
            Console.WriteLine("Producer executed!");
        }
    }
}