using Common.Interfaces;
using Common.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Consumer.Services
{
    public class OrderProcessingService : BackgroundService
    {
        private readonly IRabbitMQService _rabbitMQService;
        private readonly ILogger<OrderProcessingService> _logger;

        public OrderProcessingService(
            IRabbitMQService rabbitMQService,
            ILogger<OrderProcessingService> logger)
        {
            _rabbitMQService = rabbitMQService;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _rabbitMQService.StartConsumer(ProcessMessage);
            return Task.CompletedTask;
        }

        private void ProcessMessage(string message)
        {
            try
            {
                var order = JsonSerializer.Deserialize<OrderMessage>(message);
                _logger.LogInformation($"Processing order {order.OrderId} for customer {order.CustomerName}");

                // Thực hiện xử lý order ở đây
                // Ví dụ: lưu vào database, gửi email, etc.
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
            }
        }
    }
}
