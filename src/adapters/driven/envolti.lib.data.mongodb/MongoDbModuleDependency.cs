using envolti.lib.data.mongodb.Order;
using envolti.lib.order.domain.Order.Ports;
using Microsoft.Extensions.DependencyInjection;

namespace envolti.lib.data.mongodb
{
    public static class MongoDbModuleDependency
    {
        public static void AddMongoDbModule( this IServiceCollection services )
        {
            services.AddScoped<IOrderRepository, OrderRepositoryMongoAdapter>( );
        }
    }
}
