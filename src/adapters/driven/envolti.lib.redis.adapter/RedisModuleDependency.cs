using envolti.lib.order.domain.Order.Ports;
using envolti.lib.redis.adapter.Order;
using Microsoft.Extensions.DependencyInjection;

namespace envolti.lib.redis.adapter
{
    public static class RedisModuleDependency
    {
        public static void AddRedisModule( this IServiceCollection services )
        {
            services.AddSingleton<IOrderCacheAdapter, OrderRedisAdapter>( );
        }
    }
}
