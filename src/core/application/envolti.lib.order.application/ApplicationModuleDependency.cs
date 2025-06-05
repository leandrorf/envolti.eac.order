using envolti.lib.order.application.Order.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace envolti.lib.order.application
{
    public static class ApplicationModuleDependency
    {
        public static void AddApplicationModule( this IServiceCollection services, bool setService = true )
        {
            services.AddMediatR( cfg =>
                cfg.RegisterServicesFromAssembly( Assembly.GetExecutingAssembly( ) )
            );

            if ( setService )
            {
                services.AddHostedService<OrderService>( );
            }
        }
    }
}
