using envolti.lib.data.sqlserver;
using envolti.lib.order.application;
using envolti.lib.rabbitmq.adapter;
using envolti.lib.redis.adapter;

var builder = WebApplication.CreateBuilder( args );

builder.Services.AddControllers( );

builder.Services.AddEndpointsApiExplorer( );
builder.Services.AddSwaggerGen( );

builder.Services.AddApplicationModule( setService: false );
builder.Services.AddRabbitMqQueueModule( );
builder.Services.AddRedisModule( );
builder.Services.AddSqlServerModule( builder.Configuration.GetConnectionString( "Main" ) );

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
