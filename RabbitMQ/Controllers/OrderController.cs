using Common.Constants;
using Common.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Producer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IRabbitMQService _rabbitMQService;

        public OrderController(IRabbitMQService rabbitMQService)
        {
            _rabbitMQService = rabbitMQService;
        }

        [HttpPost]
        public IActionResult CreateOrder(OrderMessage order)   
        {
            order.OrderDate = DateTime.UtcNow;
            _rabbitMQService.PublishMessage(order, RabbitMQConstants.OrderRoutingKey);

            return Accepted(new { OrderId = order.OrderId });   
        }
    }
}
