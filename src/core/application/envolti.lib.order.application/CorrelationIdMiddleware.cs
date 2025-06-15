using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace envolti.lib.order.application
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware( RequestDelegate next )
        {
            _next = next;
        }

        public async Task Invoke( HttpContext context )
        {
            if ( !context.Request.Headers.TryGetValue( "X-Correlation-ID", out var correlationId ) )
            {
                correlationId = Guid.NewGuid( ).ToString( );
                context.Response.Headers[ "X-Correlation-ID" ] = correlationId;
            }

            using ( LogContext.PushProperty( "CorrelationId", correlationId ) )
            {
                await _next( context );
            }
        }
    }
}
