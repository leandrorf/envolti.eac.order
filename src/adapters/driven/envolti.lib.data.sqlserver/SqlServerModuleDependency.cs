using envolti.lib.data.sqlserver.Order;
using envolti.lib.order.domain.Order.Ports;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace envolti.lib.data.sqlserver
{
    public static class SqlServerModuleDependency
    {
        public static void AddSqlServerModule( this IServiceCollection services, string connectionString )
        {
            services.AddDbContext<SqlServerDbContext>(
                options => options.UseSqlServer( connectionString, providerOptions =>
                {
                    providerOptions.EnableRetryOnFailure( );
                    providerOptions.MigrationsAssembly( "envolti.lib.data.sqlserver" );
                } )
            );

            services.AddScoped<IOrderRepository, OrderRepository>( );
        }
    }
}
