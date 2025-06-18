using envolti.lib.order.application;
using envolti.lib.rabbitmq.adapter;

//var builder = WebApplication.CreateBuilder( args );

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

builder.Services.AddControllers( );
builder.Services.AddEndpointsApiExplorer( );
builder.Services.AddSwaggerGen( );

builder.AddApplicationModule( );
builder.Services.AddRabbitMqQueueModule( );

var app = builder.Build( );

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
