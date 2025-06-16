using envolti.lib.order.domain.Order.Ports;
using envolti.lib.order.domain.Order.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Sinks.Grafana.Loki;
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

        public static void AddApplicationModule( this WebApplicationBuilder builder )
        {
            AddApplicationModule( builder.Services );

            var rabbitMqSettings = builder.Configuration.GetSection( "Services:RabbitMQ" );
            var redisSettings = builder.Configuration.GetSection( "Services:Redis" );
            var sqlServerSettings = builder.Configuration.GetSection( "ConnectionStrings" );

            if ( rabbitMqSettings != null )
            {
                builder.Services.Configure<RabbitMqSettings>( rabbitMqSettings );
            }

            if ( redisSettings != null )
            {
                builder.Services.Configure<RedisSettings>( redisSettings );
            }

            if ( sqlServerSettings != null )
            {
                builder.Services.Configure<SqlServerSettigns>( sqlServerSettings );
            }

            var lokiUrl = builder.Configuration.GetSection( "Services:LokiUrl" ).Value;
            var applicationName = builder.Configuration.GetSection( "ApplicationName" ).Value;

            builder.Host.UseSerilog( ( context, services, configuration ) =>
            {
                configuration
                    .Enrich.FromLogContext( )
                    .WriteTo.Console( outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message} (CorrelationId: {CorrelationId}){NewLine}{Exception}" )
                    .WriteTo.Console( )
                    .WriteTo.GrafanaLoki( lokiUrl, labels: new List<LokiLabel>
                    {
                        new LokiLabel { Key = "api", Value = applicationName }
                    } )
                    .Enrich.WithProperty( "Application", applicationName )
                    .ReadFrom.Configuration( context.Configuration );
            } );
        }
    }
}
