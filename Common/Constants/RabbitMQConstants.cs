namespace Common.Constants
{
    // Constants/RabbitMQConstants.cs
    public static class RabbitMQConstants
    {
        public const string HostName = "localhost";
        public const string UserName = "guest";
        public const string Password = "guest";

        public const string OrderExchange = "order_exchange";
        public const string OrderQueue = "order_queue";
        public const string OrderRoutingKey = "order.created";
    }

}
