using envolti.lib.data.sqlserver;
using envolti.lib.order.application;
using envolti.lib.rabbitmq.adapter;
using envolti.lib.redis.adapter;
using envolti.service.order.driving;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var environment = Environment.GetEnvironmentVariable( "ASPNETCORE_ENVIRONMENT" ) ?? "Production";

var builder = WebApplication.CreateBuilder( new WebApplicationOptions
{
    EnvironmentName = environment
} );

builder.Configuration
    .SetBasePath( Directory.GetCurrentDirectory( ) )
    .AddJsonFile( "appsettings.json", optional: false, reloadOnChange: true )
    .AddJsonFile( $"appsettings.{environment}.json", optional: true, reloadOnChange: true )
    .AddEnvironmentVariables( );

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
