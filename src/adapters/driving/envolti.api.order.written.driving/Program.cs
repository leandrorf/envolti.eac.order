using envolti.lib.order.application;
using envolti.lib.order.application.Order.Commands;
using envolti.lib.order.application.Order.Responses;
using envolti.lib.order.domain.Order.Settings;
using envolti.lib.rabbitmq.adapter;
using MediatR;
using Serilog;
using Serilog.Sinks.Grafana.Loki;
using System.Reflection;

var builder = WebApplication.CreateBuilder( args );

builder.Services.AddControllers( );

builder.Services.AddEndpointsApiExplorer( );
builder.Services.AddSwaggerGen( );

builder.Services.AddApplicationModule( );
builder.Services.AddRabbitMqQueueModule( );

builder.Services.Configure<RabbitMqSettings>( builder.Configuration.GetSection( "Services:RabbitMQ" ) );

builder.Host.UseSerilog( ( context, services, configuration ) =>
{
    configuration
        .Enrich.FromLogContext( )
        .WriteTo.Console( outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message} (CorrelationId: {CorrelationId}){NewLine}{Exception}" )
        .WriteTo.Console( ) 
        .WriteTo.GrafanaLoki( "http://host.docker.internal:3100", labels: new List<LokiLabel>
        {
            new LokiLabel { Key = "api", Value = "order-written" }
        } ) 
        .Enrich.WithProperty( "Application", "MinhaAPI" )
        .ReadFrom.Configuration( context.Configuration );
} );

var app = builder.Build( );

// Configure the HTTP request pipeline.
if ( app.Environment.IsDevelopment( ) )
{
    app.UseSwagger( );
    app.UseSwaggerUI( );
}

app.UseMiddleware<CorrelationIdMiddleware>( );

app.UseHttpsRedirection( );

app.UseAuthorization( );

app.MapControllers( );

app.Run( );
