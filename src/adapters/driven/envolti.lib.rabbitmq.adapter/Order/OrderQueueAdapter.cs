using envolti.lib.order.domain.Order.Dtos;
using envolti.lib.order.domain.Order.Ports;
using envolti.lib.order.domain.Order.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace envolti.lib.rabbitmq.adapter.Order
{
    public class OrderQueueAdapter : IOrderQueuesAdapter, IAsyncDisposable
    {
        private IConnection _Connection = null!;
        private IChannel _Channel = null!;
        private AsyncEventingBasicConsumer _Consumer;
        private readonly SemaphoreSlim _Lock = new( 1, 1 );
        private readonly IOptions<RabbitMqSettings> _Settings;
        private readonly ILogger<OrderQueueAdapter> _Logger;

        public OrderQueueAdapter( ILogger<OrderQueueAdapter> logger, IOptions<RabbitMqSettings> settings )
        {
            _Logger = logger;
            _Settings = settings;
        }

        private async Task EnsureInitializedAsync( string queueName )
        {
            await _Lock.WaitAsync( );
            try
            {
                if ( _Channel != null && _Channel.IsOpen )
                {
                    return;
                }

                var factory = new ConnectionFactory
                {
                    HostName = _Settings.Value.Host,
                };

                _Connection = await factory.CreateConnectionAsync( );
                _Channel = await _Connection.CreateChannelAsync( );

                await _Channel.QueueDeclareAsync( queueName, true, false, false, null );
                await _Channel.BasicQosAsync( 0, 1, false );
            }
            finally
            {
                _Lock.Release( );
            }
        }

        public async Task ConsumerOrderAsync( string queueName, CancellationToken stoppingToken, Func<OrderRequestDto, Task> processOrderCallback )
        {
            await EnsureInitializedAsync( queueName );

            _Consumer = new AsyncEventingBasicConsumer( _Channel );
            _Consumer.ReceivedAsync += async ( _, ea ) =>
            {
                var message = Encoding.UTF8.GetString( ea.Body.ToArray( ) );

                try
                {
                    var order = JsonConvert.DeserializeObject<OrderRequestDto>( message );

                    if ( order != null )
                    {
                        await processOrderCallback( order );
                        await _Channel.BasicAckAsync( ea.DeliveryTag, false );
                    }
                }
                catch ( Exception ex )
                {
                    _Logger.LogError( ex, $"Erro ao processar a mensagem: {message}" );
                    await _Channel.BasicNackAsync( ea.DeliveryTag, false, true );
                }
            };

            await _Channel.BasicConsumeAsync( queue: queueName, autoAck: false, consumer: _Consumer );
        }

        public async Task<OrderRequestDto> PublishOrderAsync( OrderRequestDto order, string queueName )
        {
            await EnsureInitializedAsync( queueName );

            try
            {
                string message = JsonConvert.SerializeObject( order );
                var body = Encoding.UTF8.GetBytes( message );

                var properties = new BasicProperties( );
                properties.CorrelationId = order.OrderIdExternal.ToString( );

                await _Channel.BasicPublishAsync(
                    exchange: "",
                    routingKey: queueName,
                    mandatory: false,
                    basicProperties: properties,
                    body: body,
                    cancellationToken: CancellationToken.None
                );

                _Logger.LogInformation( $"Pedido {order.OrderIdExternal} publicado na fila '{queueName}'. Request Body: {order}", order );

                return order;
            }
            catch ( Exception ex )
            {
                _Logger.LogError( ex, $"Erro ao publicar na fila '{queueName}': {ex.Message}" );
                throw new Exception( $"Failed to publish message to queue '{queueName}': {ex.Message}", ex );
            }
        }

        public async Task CloseConnectionAsync( )
        {
            if ( _Channel != null )
            {
                await _Channel.CloseAsync( );
            }

            if ( _Connection != null )
            {
                await _Connection.CloseAsync( );
            }
        }

        public async Task<bool> Exists( string queueName, int correlationId )
        {
            await EnsureInitializedAsync( queueName );

            if ( _Channel == null || !_Channel.IsOpen )
            {
                _Logger.LogWarning( "Canal não está aberto. Retornando falso." );
                return false;
            }

            var resp = await _Channel.BasicGetAsync( queueName, false );

            if ( resp != null && resp.BasicProperties.CorrelationId == correlationId.ToString( ) )
            {
                return true;
            }

            _Logger.LogInformation( $"O objeto com CorrelationId {correlationId} não existe na fila '{queueName}'." );

            return false;
        }

        public async ValueTask DisposeAsync( )
        {
            _Logger.LogInformation( "Fechando conexão RabbitMQ..." );

            if ( _Channel?.IsOpen == true )
            {
                await _Channel.CloseAsync( );
            }
            if ( _Connection?.IsOpen == true )
            {
                await _Connection.CloseAsync( );
            }
        }
    }
}
