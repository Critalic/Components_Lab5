using System;
using System.IO;
using System.Threading;

using Consumer.Interfaces;

using Lab.Shared.Configuration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Consumer
{
    internal class Program
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
                    // Adding consumer as a singleton service
                    services.AddSingleton<IConsumer, Consumer>();
                    
                    // Configuring consumer`s options (converting appsettings.json to C# class object)
                    services.Configure<ConsumerOptions>(hostingContext.Configuration.GetSection("Consumer"));
                })
                .Build();
            
            // Creating a Consumer instance
            var logsConsumer = ActivatorUtilities.CreateInstance<Consumer>(host.Services);
            
            // Creating a separated thread for consumer
            var consumerThread = new Thread(() => logsConsumer.ExecuteProcessing())
            {
                Name = "Consumer",
                IsBackground = false,
                Priority = ThreadPriority.Normal
            };
            consumerThread.Start();
            Console.WriteLine("Consumer executed!");
        }
    }
}
