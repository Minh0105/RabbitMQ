namespace Common.Interfaces
{
    // Interfaces/IRabbitMQService.cs
    public interface IRabbitMQService
    {
        void PublishMessage<T>(T message, string routingKey);
        void StartConsumer(Action<string> processMessage);
    }
}
