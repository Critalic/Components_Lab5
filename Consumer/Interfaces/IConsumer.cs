namespace Consumer.Interfaces
{
    /// <summary>
    /// Base interface for the consumer. Required for DI
    /// </summary>
    public interface IConsumer
    {
        void ExecuteProcessing();
    }
}