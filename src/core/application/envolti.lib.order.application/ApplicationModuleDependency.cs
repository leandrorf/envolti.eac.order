﻿using envolti.lib.order.application.Mediator;
using envolti.lib.order.application.Mediator.Interfaces;
using envolti.lib.order.domain.Order.Ports;
using envolti.lib.order.domain.Order.Settings;
using Microsoft.AspNetCore.Builder;
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
            var assembly = Assembly.GetExecutingAssembly( );

            var handlerTypes = assembly.GetTypes( )
                .Where( t => !t.IsAbstract && !t.IsInterface )
                .SelectMany( t => t.GetInterfaces( ), ( type, iface ) => new { type, iface } )
                .Where( t => t.iface.IsGenericType && t.iface.GetGenericTypeDefinition( ) == typeof( IRequestHandler<,> ) );

            foreach ( var handler in handlerTypes )
            {
                services.AddScoped( handler.iface, handler.type );
            }

            services.AddScoped<IMediator, MediatorLR>( );

            services.AddScoped<IOrderRepository>( sp =>
            {
                throw new InvalidOperationException( "A implementação de IOrderRepository deve ser registrada no projeto de envolti.lib.data.sqlserver." );
            } );

            services.AddScoped<IOrderCacheAdapter>( sp =>
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
            var mongoSettings = builder.Configuration.GetSection( "MongoSettings" );
            var applicationName = builder.Configuration.GetSection( "ApplicationSettings:ApplicationName" ).Value;
            var applicationType = builder.Configuration.GetSection( "ApplicationSettings:ApplicationType" ).Value;
            var lokiUrl = builder.Configuration.GetSection( "Services:LokiUrl" ).Value;

            if ( rabbitMqSettings != null )
            {
                builder.Services.Configure<RabbitMqSettings>( rabbitMqSettings );
            }

            if ( redisSettings != null )
            {
                builder.Services.Configure<RedisSettings>( redisSettings );
            }

            if ( mongoSettings != null )
            {
                builder.Services.Configure<MongoSettings>( mongoSettings );
            }

            builder.Host.UseSerilog( ( context, services, configuration ) =>
            {
                configuration
                    .Enrich.FromLogContext( )
                    .Enrich.With<LogIdEnricher>( )
                    .Enrich.With<TimestampJitterEnricher>( )
                    .Enrich.WithProperty( "Application", applicationName )
                    .Enrich.WithProperty( "Type", applicationType )
                    .WriteTo.Console( outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message} (LogId: {LogId}){NewLine}{Exception}" )
                    .WriteTo.GrafanaLoki(
                        lokiUrl!,
                        textFormatter: new NoMessageTemplateJsonFormatter( ), // Garante logs estruturados
                        labels: new List<LokiLabel>
                        {
                            new LokiLabel { Key = applicationType!, Value = applicationName! }
                        } )
                    .ReadFrom.Configuration( context.Configuration );
            } );
        }
    }
}
