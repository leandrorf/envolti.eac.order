using Serilog;
using Serilog.Context;

namespace envolti.api.order.written.driving
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
            // Captura ou gera Correlation ID
            if ( !context.Request.Headers.TryGetValue( "X-Correlation-ID", out var correlationId ) )
            {
                correlationId = Guid.NewGuid( ).ToString( );
                context.Response.Headers[ "X-Correlation-ID" ] = correlationId;
            }

            // Ativa Buffering para ler o Body
            context.Request.EnableBuffering( );

            using var reader = new StreamReader( context.Request.Body, leaveOpen: true );
            var body = await reader.ReadToEndAsync( );
            context.Request.Body.Position = 0; // Restaura posição do Body

            //// Cria objeto JSON para log
            //var logData = new
            //{
            //    CorrelationId = correlationId,
            //    Method = context.Request.Method,
            //    Path = context.Request.Path,
            //    RequestBody = body
            //};

            // Insere CorrelationId no LogContext e loga a requisição
            using ( LogContext.PushProperty( "CorrelationId", correlationId ) )
            {
                // Logando tudo na mesma linha
                Log.Information( "CorrelationId: {CorrelationId} | Method: {Method} | Path: {Path} | Body: {Body}",
                    correlationId, context.Request.Method, context.Request.Path, body );


                await _next( context );
            }

        }
    }
}
