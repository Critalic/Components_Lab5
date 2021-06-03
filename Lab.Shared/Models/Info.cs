using System;

namespace Lab.Shared.Models
{
    /// <summary>
    /// Represents object which will be pushed to the queue
    /// </summary>
    public class Info
    {
        public string Sender { get; set; }
        public DateTime SendingTimeStamp { get; set; }
    }
}