using Serilog.Core;
using Serilog.Events;

namespace envolti.lib.order.application
{
    public class LogIdEnricher : ILogEventEnricher
    {
        public void Enrich( LogEvent logEvent, ILogEventPropertyFactory propertyFactory )
        {
            var logId = Guid.NewGuid( ).ToString( );
            logEvent.AddPropertyIfAbsent( propertyFactory.CreateProperty( "LogId", logId ) );
        }
    }
}
