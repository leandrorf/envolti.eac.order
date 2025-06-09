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
        }
    }
}
