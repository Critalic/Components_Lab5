using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using Lab.Shared.Configuration;
using Lab.Shared.Models;

using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using Producer.Interfaces;

using RabbitMQ.Client;

namespace Producer
{
    /// <summary>
    /// Base <see cref="IProducer"/> implementation. Pushes data to the queue 
    /// </summary>
    public class Producer : IProducer
    {
        private readonly ProducerOptions _producerOptions;
        
        public Producer(IOptions<ProducerOptions> options)
        {
            _producerOptions = options.Value;
        }

        public void ExecuteSending()
        {
            // Creating a connection factory (object which will create connections for the producer)
            var factory = new ConnectionFactory() { HostName = _producerOptions.RabbitMqHost };
            // Connection object (unmanaged resource) 
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            
            // Creating a queue where info would be pushed (if not exists, else nothing happen)
            channel.QueueDeclare(queue: "logs_queue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            while (true)
            {
                // Info which will be published to the queue
                var infoPiece = new Info()
                {
                    Sender = _producerOptions.Sender,
                    SendingTimeStamp = DateTime.Now,
                };

                // Converting info to JSON representation
                var infoJson = JsonConvert.SerializeObject(infoPiece);

                // Converting info JSON representation to byte array representation
                var body = Encoding.UTF8.GetBytes(infoJson);
                channel.BasicPublish(exchange: "", routingKey: "logs_queue", basicProperties: null, body: body);

                // Pause info pushing
                Thread.Sleep(_producerOptions.ReportsSendingInterval);
            }
        }
    }
}