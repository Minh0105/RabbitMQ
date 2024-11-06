using Common.Constants;
using Common.Interfaces;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consumer.Services
{
    public class ConsumerRabbitMQService : IRabbitMQService, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ILogger<ConsumerRabbitMQService> _logger;

        public ConsumerRabbitMQService(ILogger<ConsumerRabbitMQService> logger)
        {
            _logger = logger;

            var factory = new ConnectionFactory
            {
                HostName = RabbitMQConstants.HostName,
                UserName = RabbitMQConstants.UserName,
                Password = RabbitMQConstants.Password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Declare Exchange và Queue giống như Producer
            _channel.ExchangeDeclare(
                RabbitMQConstants.OrderExchange,
                ExchangeType.Topic,
                durable: true,
                autoDelete: false
            );

            _channel.QueueDeclare(
                RabbitMQConstants.OrderQueue,
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            _channel.QueueBind(
                RabbitMQConstants.OrderQueue,
                RabbitMQConstants.OrderExchange,
                RabbitMQConstants.OrderRoutingKey
            );
        }

        public void PublishMessage<T>(T message, string routingKey)
        {
            throw new NotImplementedException();
        }

        public void StartConsumer(Action<string> processMessage)
        {
            _channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (sender, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    processMessage(message);
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message");
                    _channel.BasicNack(ea.DeliveryTag, false, true);
                }
            };

            _channel.BasicConsume(
                queue: RabbitMQConstants.OrderQueue,
                autoAck: false,
                consumer: consumer
            );
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
