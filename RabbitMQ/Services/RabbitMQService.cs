using Microsoft.AspNetCore.Connections;
using System.Text.Json;
using System.Text;
using RabbitMQ.Client;
using Common.Interfaces;
using Common.Constants;

namespace Producer.Services
{
    public class RabbitMQService : IRabbitMQService, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMQService()
        {
            var factory = new ConnectionFactory
            {
                HostName = RabbitMQConstants.HostName,
                UserName = RabbitMQConstants.UserName,
                Password = RabbitMQConstants.Password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Declare Exchange
            _channel.ExchangeDeclare(
                RabbitMQConstants.OrderExchange,
                ExchangeType.Topic,
                durable: true,
                autoDelete: false
            );

            // Declare Queue
            _channel.QueueDeclare(
                RabbitMQConstants.OrderQueue,
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            // Bind Queue to Exchange
            _channel.QueueBind(
                RabbitMQConstants.OrderQueue,
                RabbitMQConstants.OrderExchange,
                RabbitMQConstants.OrderRoutingKey
            );
        }

        public void PublishMessage<T>(T message, string routingKey)
        {
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;

            _channel.BasicPublish(
                RabbitMQConstants.OrderExchange,
                routingKey,
                properties,
                body
            );
        }

        public void StartConsumer(Action<string> processMessage)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }

}
