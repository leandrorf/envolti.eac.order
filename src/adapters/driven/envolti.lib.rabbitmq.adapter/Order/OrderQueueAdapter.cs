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
            int delayMilliseconds = 20000;

            for ( int i = 0; i < retryCount; i++ )
            {
                try
                {
                    if ( _Connection == null || !_Connection.IsOpen )
                    {
                        var factory = new ConnectionFactory
                        {
                            HostName = "localhost",
                            RequestedHeartbeat = TimeSpan.FromSeconds( 30 ),
                            NetworkRecoveryInterval = TimeSpan.FromSeconds( 10 )
                        };

                        _Connection = await factory.CreateConnectionAsync( );
                        _Channel = await _Connection.CreateChannelAsync(
                            new CreateChannelOptions( true, true )
                        );
                    }

                    if ( _Channel == null )
                    {
                        throw new InvalidOperationException( "Failed to initialize RabbitMQ channel." );
                    }

                    return;
                }
                catch ( Exception ex )
                {
                    Console.WriteLine( $"Error initializing RabbitMQ (attempt {i + 1}): {ex.Message}" );
                    await Task.Delay( delayMilliseconds );
                }
            }

            throw new Exception( "Failed to connect to RabbitMQ after multiple attempts." );
        }

        private async Task EnsureInitializedAsync( string queueName )
        {
            try
            {
                await _initTask.Value;

                await _Channel.QueueDeclareAsync(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                await _Channel.BasicQosAsync( 0, 1, false );
            }
            catch ( Exception ex )
            {
                Console.WriteLine( "RabbitMq initialization failed." );

                await DisposeAsync( );
                await Task.Delay( 5000 );
                await InitAsync( );
                await EnsureInitializedAsync( queueName );
            }
        }

        public async Task ConsumerOrderAsync( string queueName, Func<OrderRequestDto, Task> processOrderCallback, CancellationToken stoppingToken )
        {
            int countMessage = 0;
            int intervalDelay = 1000;

            await EnsureInitializedAsync( queueName );

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

                countMessage++;

                if ( countMessage % intervalDelay == 0 )
                {
                    Console.WriteLine( "Delay intencional após processar múltiplas mensagens..." );
                    await Task.Delay( 10000 );
                }
            };

            await _Channel.BasicConsumeAsync(
                queue: queueName,
                autoAck: false,
                consumer: consumer
            );

            _ = MonitorConnectionAsync( stoppingToken );

            while ( !stoppingToken.IsCancellationRequested )
            {
                await Task.Delay( 60000, stoppingToken );
            }
        }

        public async Task<OrderRequestDto> PublishOrderAsync( OrderRequestDto order, string queueName )
        {
            await EnsureInitializedAsync( queueName );

            //// Verifica se a conexão está ativa
            //if ( _Channel == null || !_Channel.IsOpen )
            //{
            //    Console.WriteLine( "Canal não está aberto. Tentando reconectar..." );
            //    await InitAsync( );
            //}

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

                Console.WriteLine( $"Pedido publicado na fila '{queueName}' com sucesso!" );

                return order;
            }
            catch ( Exception ex )
            {
                Console.WriteLine( $"Erro ao publicar na fila '{queueName}': {ex.Message}" );
                throw;
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
                Console.WriteLine( "Canal não está aberto. Retornando falso." );
                return false;
            }

            var resp = await _Channel.BasicGetAsync( queueName, false );

            if ( resp != null && resp.BasicProperties.CorrelationId == correlationId.ToString( ) )
            {
                return true;
            }

            Console.WriteLine( "O objeto já existe na fila." );

            return false;
        }

        private async Task MonitorConnectionAsync( CancellationToken stoppingToken )
        {
            while ( !stoppingToken.IsCancellationRequested )
            {
                if ( _Connection == null || !_Connection.IsOpen || _Channel == null || !_Channel.IsOpen )
                {
                    Console.WriteLine( "Conexão perdida. Tentando reconectar..." );
                    await Task.Delay( 30000, stoppingToken );
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
