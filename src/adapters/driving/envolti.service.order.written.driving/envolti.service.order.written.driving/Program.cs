using envolti.lib.data.mongodb;
using envolti.lib.order.application;
using envolti.lib.rabbitmq.adapter;
using envolti.lib.redis.adapter;
using envolti.service.order.driving;
using Microsoft.AspNetCore.Builder;

var environment = Environment.GetEnvironmentVariable( "ASPNETCORE_ENVIRONMENT" ) ?? "Production";

var builder = WebApplication.CreateBuilder( new WebApplicationOptions
{
    EnvironmentName = environment
} );

//var builder = Host.CreateApplicationBuilder( args );

builder.Configuration
    .SetBasePath( Directory.GetCurrentDirectory( ) )
    .AddJsonFile( "appsettings.json", optional: false, reloadOnChange: true )
    .AddJsonFile( $"appsettings.{environment}.json", optional: true, reloadOnChange: true )
    .AddEnvironmentVariables( );

builder.Services.AddHostedService<Worker>( );

builder.AddApplicationModule( );
builder.Services.AddRabbitMqQueueModule( );
builder.Services.AddRedisModule( );
builder.Services.AddMongoDbModule( );
//builder.Services.AddSqlServerModule( builder.Configuration.GetConnectionString( "Default" ) );

var host = builder.Build( );

//using ( var scope = host.Services.CreateScope( ) )
//{
//    var context = scope.ServiceProvider.GetRequiredService<SqlServerDbContext>( );
//    context.Database.Migrate( );
//}

host.Run( );
