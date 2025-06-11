using envolti.lib.data.sqlserver;
using envolti.lib.order.application;
using envolti.lib.rabbitmq.adapter;
using envolti.lib.redis.adapter;
using envolti.service.order.driving;
using System.Reflection;

var builder = Host.CreateApplicationBuilder( args );
builder.Services.AddHostedService<Worker>( );

builder.Services.AddMediatR( cfg =>
    cfg.RegisterServicesFromAssembly( Assembly.GetExecutingAssembly( ) )
);

builder.Services.AddApplicationModule( );
builder.Services.AddRabbitMqQueueModule( );
builder.Services.AddRedisModule( );
builder.Services.AddSqlServerModule( builder.Configuration.GetConnectionString( "Main" ) );

var host = builder.Build( );

host.Run( );
