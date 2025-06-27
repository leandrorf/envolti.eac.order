using Serilog.Core;
using Serilog.Events;

namespace envolti.lib.order.application
{
    public class TimestampJitterEnricher : ILogEventEnricher
    {
        private static readonly Random _random = new( );

        public void Enrich( LogEvent logEvent, ILogEventPropertyFactory propertyFactory )
        {
            var jitter = _random.Next( 1, 1000 ); // até 999 ticks
            var jitteredTicks = DateTime.UtcNow.Ticks + jitter;
            var jitteredTime = new DateTime( jitteredTicks, DateTimeKind.Utc );

            logEvent.AddPropertyIfAbsent( propertyFactory.CreateProperty( "TimestampJitter", jitteredTime ) );
        }
    }
}
