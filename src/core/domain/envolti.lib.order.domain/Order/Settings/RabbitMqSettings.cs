namespace envolti.lib.order.domain.Order.Settings
{
    public class RabbitMqSettings
    {
        public string? Host { get; set; }
        public int Port { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? VirtualHost { get; set; }
        public Queues? Queue { get; set; }

        public class Queues
        {
            public string? OrderQueue { get; set; }
        }
    }
}
