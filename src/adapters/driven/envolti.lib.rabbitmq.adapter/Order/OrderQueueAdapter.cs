using envolti.lib.order.domain.Order.Dtos;
using envolti.lib.order.domain.Order.Ports;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Diagnostics;
using System.Text;


namespace envolti.lib.rabbitmq.adapter.Order
{
    public class OrderQueueAdapter : IOrderQueuesAdapter, IAsyncDisposable
    {
        private IConnection _Connection = null!;
        private IChannel _Channel = null!;
        private readonly Lazy<Task> _initTask;

        public OrderQueueAdapter( )
        {
            _initTask = new Lazy<Task>( InitAsync );
        }

        public async Task InitAsync( )
        {
            if ( _Channel != null && _Channel.IsOpen )
            {
                return;
            }

            int retryCount = 5;
            int delayMilliseconds = 2000;

            for ( int i = 0; i < retryCount; i++ )
            {
                try
                {
                    if ( _Connection == null || !_Connection.IsOpen )
                    {
                        var factory = new ConnectionFactory
                        {
                            HostName = "localhost",
                            RequestedHeartbeat = TimeSpan.FromSeconds( 30 )
                        };

                        _Connection = await factory.CreateConnectionAsync( );
                        _Channel = await _Connection.CreateChannelAsync( );
                    }
                    return;
                }
                catch ( Exception ex )
                {
                    Console.WriteLine( $"Erro ao inicializar RabbitMQ (tentativa {i + 1}): {ex.Message}" );
                    await Task.Delay( delayMilliseconds );
                }
            }

            throw new Exception( "Falha ao conectar ao RabbitMQ após múltiplas tentativas." );
        }

        private async Task EnsureInitializedAsync( )
        {
            try
            {
                await _initTask.Value;
            }
            catch ( Exception ex )
            {
                Console.WriteLine( $"Erro ao inicializar Redis: {ex.Message}" );
                throw;
            }
        }

        public async Task ConsumerOrderAsync( string queueName, Func<OrderRequestDto, Task> processOrderCallback, CancellationToken stoppingToken )
        {
            await EnsureInitializedAsync( );

            await _Channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var consumer = new AsyncEventingBasicConsumer( _Channel );

            consumer.ReceivedAsync += async ( model, ea ) =>
            {
                Stopwatch stopwatch = Stopwatch.StartNew( );

                try
                {
                    var mensagem = Encoding.UTF8.GetString( ea.Body.ToArray( ) );
                    var deserializedOrder = JsonConvert.DeserializeObject<OrderRequestDto>( mensagem );

                    if ( deserializedOrder != null )
                    {
                        await processOrderCallback( deserializedOrder );
                        await _Channel.BasicAckAsync( ea.DeliveryTag, false );
                    }
                }
                catch ( Exception ex )
                {
                    Console.WriteLine( $"Erro ao processar mensagem: {ex.Message}" );
                    await _Channel.BasicNackAsync( ea.DeliveryTag, false, true );
                }

                stopwatch.Stop( );
                Console.WriteLine( $"Tempo total do processamento do pedido: {stopwatch.ElapsedMilliseconds} ms" );
            };

            await _Channel.BasicConsumeAsync(
                queue: queueName,
                autoAck: false,
                consumer: consumer
            );

            _ = MonitorConnectionAsync( stoppingToken );

            while ( !stoppingToken.IsCancellationRequested )
            {
                await Task.Delay( 1000, stoppingToken );
            }
        }

        public async Task<OrderRequestDto> PublishOrderAsync( OrderRequestDto order, string queueName )
        {
            await EnsureInitializedAsync( );

            await _Channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

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

            return order;
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
            await EnsureInitializedAsync( );

            if ( _Channel == null || !_Channel.IsOpen )
            {
                Console.WriteLine( "Canal não está aberto. Retornando falso." );
                return false;
            }

            var result = await _Channel.BasicGetAsync( queueName, false );

            return result != null && result.BasicProperties.CorrelationId == correlationId.ToString( );
        }

        private async Task MonitorConnectionAsync( CancellationToken stoppingToken )
        {
            while ( !stoppingToken.IsCancellationRequested )
            {
                if ( _Connection == null || !_Connection.IsOpen || _Channel == null || !_Channel.IsOpen )
                {
                    Console.WriteLine( "Conexão perdida. Tentando reconectar..." );
                    await InitAsync( );
                }

                await Task.Delay( 5000, stoppingToken );
            }
        }

        public async ValueTask DisposeAsync( )
        {
            await CloseConnectionAsync( );
        }
    }
}
