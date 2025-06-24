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
            var message = logEvent.MessageTemplate?.Text;
            if ( !string.IsNullOrEmpty( message ) && logEvent.Properties != null && logEvent.Properties.Count > 0 )
            {
                // Substitui os placeholders pelos valores
                foreach ( var kv in logEvent.Properties )
                {
                    message = message.Replace( "{" + kv.Key + "}", kv.Value.ToString( ).Trim( '"' ) );
                }
            }

            output.Write( "{" );
            output.Write( $"\"Timestamp\":\"{logEvent.Timestamp:O}\"" );
            output.Write( $",\"Level\":\"{logEvent.Level}\"" );
            output.Write( $",\"Message\":\"{message}\"" );
            // NÃO escreve MessageTemplate
            if ( logEvent.Properties.Any( ) )
            {
                output.Write( ",\"Properties\":{" );
                output.Write( string.Join( ",", logEvent.Properties.Select( kv => $"\"{kv.Key}\":{kv.Value}" ) ) );
                output.Write( "}" );
            }
            output.Write( "}" );
        }
    }
}
