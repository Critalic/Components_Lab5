namespace Lab.Shared.Configuration
{
    public class ConsumerOptions
    {
        public const string Consumer = "Consumer";
        
        public int LogsRequestInterval { get; set; }
        public string RabbitMqHost { get; set; }
        public TelegramBotOptions TelegramBot { get; set; }
    }
}