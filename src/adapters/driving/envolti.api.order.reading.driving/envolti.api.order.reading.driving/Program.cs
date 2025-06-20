using envolti.lib.data.mongodb;
using envolti.lib.order.application;
using envolti.lib.redis.adapter;
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

builder.Services.AddControllers( );
builder.Services.AddEndpointsApiExplorer( );
builder.Services.AddSwaggerGen( );

builder.Services.AddMediatR( cfg =>
    cfg.RegisterServicesFromAssembly( Assembly.GetExecutingAssembly( ) )
);

builder.AddApplicationModule( );
builder.Services.AddRedisModule( );
builder.Services.AddMongoDbModule( );
//builder.Services.AddSqlServerModule( builder.Configuration.GetConnectionString( "Default" ) );

var corsPolicyName = "AllowAllOrigins";

builder.Services.AddCors( options =>
{
    options.AddPolicy( name: corsPolicyName,
        policy =>
        {
            policy.AllowAnyOrigin( )
                  .AllowAnyMethod( )
                  .AllowAnyHeader( );
        } );
} );

//builder.WebHost.UseUrls( "http://*:8084" );

var app = builder.Build( );

// Configure the HTTP request pipeline.
if ( app.Environment.IsDevelopment( ) )
{
    app.UseSwagger( );
    app.UseSwaggerUI( );
}

app.UseHttpsRedirection( );

app.UseCors( corsPolicyName );

app.UseAuthorization( );

app.MapControllers( );

app.Run( );
