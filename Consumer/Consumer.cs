using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Consumer.Interfaces;

using Lab.Shared.Configuration;
using Lab.Shared.Models;

using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Consumer
{
    /// <summary>
    /// Base <see cref="IConsumer"/> implementation
    /// </summary>
    public class Consumer : IConsumer
    {
        private readonly ConsumerOptions _consumerOptions;
        private readonly ITelegramBotClient _botClient;

        public Consumer(IOptions<ConsumerOptions> consumerOptions)
        {
            _consumerOptions = consumerOptions.Value;
            _botClient = new TelegramBotClient(_consumerOptions.TelegramBot.Token);
        }

        public void ExecuteProcessing()
        {
            // Creating a connection factory (object which will create connections for the consumer)
            var factory = new ConnectionFactory() { HostName = _consumerOptions.RabbitMqHost };
            // Connection object (unmanaged resource) 
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            
            // Creating a queue where info would be pushed (if not exists, else nothing happen)
            channel.QueueDeclare(queue: "logs_queue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            // Creating a consumer
            var consumer = new EventingBasicConsumer(channel);
            
            // Creating event handler, calls when each piece of info received
            consumer.Received += async (_, ea) =>
            {
                // Converting info received from the queue to byte array 
                var body = ea.Body.ToArray();
                
                // Converting received info to JSON
                var json = Encoding.UTF8.GetString(body);
                
                // Converting JSON to C# class object
                var info = JsonConvert.DeserializeObject<Info>(json);
                
                // Publish received info to Telegram
                await PublishInfoToTg(info);
                Console.WriteLine(
                    $" [{DateTime.Now:F}] Received:\n*{info.Sender}*\n\r\n\r{info.SendingTimeStamp.ToLongTimeString()}");
            };

            while (true)
            {
                // Requesting a piece of info from the queue
                channel.BasicConsume(queue: "logs_queue",
                    autoAck: true,
                    consumer: consumer);
                
                // Pause thread
                Thread.Sleep(_consumerOptions.LogsRequestInterval);
            }
        }

        /// <summary>
        /// Publish info to Telegram
        /// </summary>
        /// <param name="info">Info to publish</param>
        private async Task PublishInfoToTg(Info info)
        {
            if (info != null)
            {
                await _botClient.SendTextMessageAsync(_consumerOptions.TelegramBot.ChatId, 
                    $"*{info.Sender}*\n\r\n\r{info.SendingTimeStamp.ToLongTimeString()}", 
                    ParseMode.Markdown);
            }
        }
    }
}