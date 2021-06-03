namespace Lab.Shared.Configuration
{
    public class TelegramBotOptions
    {
        public const string TelegramBot = "TelegramBot";
        
        public string Token { get; set; }
        public long ChatId { get; set; }
    }
}