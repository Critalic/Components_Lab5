namespace Producer.Interfaces
{
    /// <summary>
    /// Base interface for the producer. Required for DI
    /// </summary>
    public interface IProducer
    {
        void ExecuteSending();
    }
}