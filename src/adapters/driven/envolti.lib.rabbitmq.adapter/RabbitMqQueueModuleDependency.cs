using envolti.lib.order.domain.Order.Ports;
using envolti.lib.rabbitmq.adapter.Order;
using Microsoft.Extensions.DependencyInjection;

namespace envolti.lib.rabbitmq.adapter
{
    public static class RabbitMqQueueModuleDependency
    {
        public static void AddRabbitMqQueueModule( this IServiceCollection services )
        {
            services.AddTransient<IOrderQueuesAdapter, OrderQueueAdapter>( );
        }
    }
}
