using envolti.lib.order.application;
using envolti.lib.rabbitmq.adapter;

var builder = WebApplication.CreateBuilder( args );

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
