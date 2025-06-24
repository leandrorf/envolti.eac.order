using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Json;
using Serilog.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace envolti.lib.order.application
{
    public class NoMessageTemplateJsonFormatter : ITextFormatter
    {
        private readonly JsonFormatter _baseFormatter = new JsonFormatter( );

        public void Format( LogEvent logEvent, TextWriter output )
        {
            // Cria novo LogEvent sem MessageTemplate
            var logEventWithoutTemplate = new LogEvent(
                logEvent.Timestamp,
                logEvent.Level,
                logEvent.Exception,
                // Template vazio, pois não será serializado
                new MessageTemplate( "", new List<MessageTemplateToken>( ) ),
                logEvent.Properties.Select( kv => new LogEventProperty( kv.Key, kv.Value ) )
            );

            _baseFormatter.Format( logEventWithoutTemplate, output );
        }
    }
}
