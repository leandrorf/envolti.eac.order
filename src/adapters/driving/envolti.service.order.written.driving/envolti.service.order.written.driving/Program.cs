using envolti.lib.data.sqlserver;
using envolti.lib.order.application;
using envolti.lib.rabbitmq.adapter;
using envolti.lib.redis.adapter;
using envolti.service.order.driving;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

//var builder = Host.CreateApplicationBuilder( args );

var builder = WebApplication.CreateBuilder( args );

builder.Services.AddHostedService<Worker>( );

builder.Services.AddMediatR( cfg =>
    cfg.RegisterServicesFromAssembly( Assembly.GetExecutingAssembly( ) )
);

builder.AddApplicationModule( );
builder.Services.AddRabbitMqQueueModule( );
builder.Services.AddRedisModule( );
builder.Services.AddSqlServerModule( builder.Configuration.GetConnectionString( "Default" ) );

var host = builder.Build( );

using ( var scope = host.Services.CreateScope( ) )
{
    var context = scope.ServiceProvider.GetRequiredService<SqlServerDbContext>( );
    context.Database.Migrate( );
}

host.Run( );
