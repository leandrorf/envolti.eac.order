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
                options => options.UseSqlServer(connectionString, providerOptions =>
                {
                    providerOptions.EnableRetryOnFailure( );

                }
                //connectionString,
                //sqlOptions =>
                //{
                //    sqlOptions.EnableRetryOnFailure(
                //        maxRetryCount: 5,
                //        maxRetryDelay: TimeSpan.FromSeconds( 5 ),
                //        errorNumbersToAdd: null );
                //    sqlOptions.ExecutionStrategy( context => new CustomSqlExecutionStrategy( context ) );
                //    sqlOptions.MigrationsAssembly( "envolti.lib.data.sqlserver" );
                //    sqlOptions.CommandTimeout( 60 );
                //}
                )
            );

            services.AddScoped<IOrderRepository, OrderRepository>( );
        }
    }
}
