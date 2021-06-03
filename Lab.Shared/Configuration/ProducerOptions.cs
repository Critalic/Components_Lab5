namespace Lab.Shared.Configuration
{
    /// <summary>
    /// Producer configuration class
    /// </summary>
    public class ProducerOptions
    {
        public const string Producer = "Producer";

        /// <summary>
        /// The interval between sending logs in milliseconds
        /// </summary>
        public int ReportsSendingInterval { get; set; }

        /// <summary>
        /// Message broker host
        /// </summary>
        public string RabbitMqHost { get; set; }

        /// <summary>s
        /// Producer understandable string representation
        /// </summary>
        public string Sender { get; set; }
    }
}