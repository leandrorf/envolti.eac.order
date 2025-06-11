using envolti.lib.order.application;
using envolti.lib.order.application.Order.Commands;
using envolti.lib.order.application.Order.Responses;
using envolti.lib.rabbitmq.adapter;
using MediatR;
using System.Reflection;

var builder = WebApplication.CreateBuilder( args );

builder.Services.AddControllers( );

builder.Services.AddEndpointsApiExplorer( );
builder.Services.AddSwaggerGen( );

builder.Services.AddApplicationModule( );
builder.Services.AddRabbitMqQueueModule( );

var app = builder.Build( );

// Configure the HTTP request pipeline.
if ( app.Environment.IsDevelopment( ) )
{
    app.UseSwagger( );
    app.UseSwaggerUI( );
}

app.UseHttpsRedirection( );

app.UseAuthorization( );

app.MapControllers( );

app.Run( );
