using envolti.lib.order.application.Order.Commands;
using envolti.lib.order.domain.Order.Ports;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace envolti.lib.order.application
{
    public static class ApplicationModuleDependency
    {
        public static void AddApplicationModule( this IServiceCollection services )
        {
            services.AddMediatR( cfg =>
                cfg.RegisterServicesFromAssembly( Assembly.GetExecutingAssembly( ) )
            );

            services.AddScoped<IOrderRepository>( sp =>
            {
                throw new InvalidOperationException( "A implementação de IOrderRepository deve ser registrada no projeto de envolti.lib.data.sqlserver." );
            } );

            services.AddScoped<IOrderRedisAdapter>( sp =>
            {
                throw new InvalidOperationException( "A implementação de IOrderRedisAdapter deve ser registrada no projeto de envolti.lib.redis.adapter." );
            } );

            services.AddScoped<IOrderQueuesAdapter>( sp =>
            {
                throw new InvalidOperationException( "A implementação de IOrderQueuesAdapter deve ser registrada no projeto de envolti.lib.rabbitmq.adapter." );
            } );
        }
    }
}
