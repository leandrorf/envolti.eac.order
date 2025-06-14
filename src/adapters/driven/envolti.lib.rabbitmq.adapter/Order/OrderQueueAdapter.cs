using envolti.lib.order.domain.Order.Dtos;
using envolti.lib.order.domain.Order.Ports;
using envolti.lib.order.domain.Order.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        private readonly IOptions<RabbitMqSettings> _Settings;
        private readonly ILogger<OrderQueueAdapter> _Logger;
        private static readonly SemaphoreSlim _semaphore = new( 1, 1 );


        public OrderQueueAdapter( ILogger<OrderQueueAdapter> logger, IOptions<RabbitMqSettings> settings )
        {
            _initTask = new Lazy<Task>( InitAsync );
            _Logger = logger ?? throw new ArgumentNullException( nameof( logger ) );
            _Settings = settings ?? throw new ArgumentNullException( nameof( settings ) );
        }

        public async Task InitAsync( )
        {
            _Logger.LogInformation( "Iniciado serviço do RabbitMQ" );

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
                            HostName = _Settings.Value.Host,
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
                        _Logger.LogError( "Falha ao criar o canal RabbitMQ." );
                        throw new InvalidOperationException( "Failed to initialize RabbitMQ channel." );
                    }

                    return;
                }
                catch ( Exception ex )
                {
                    _Logger.LogError( ex, $"Erro ao conectar ao RabbitMQ (tentativa {i + 1}): {ex.Message}" );
                    await Task.Delay( delayMilliseconds );
                }
            }

            _Logger.LogError( "Falha ao conectar ao RabbitMQ após várias tentativas." );
            throw new Exception( "Failed to connect to RabbitMQ after multiple attempts." );
        }

        private async Task EnsureInitializedAsync( string queueName )
        {
            await _semaphore.WaitAsync( );

            try
            {
                await _initTask.Value;

                if ( _Channel == null || !_Channel.IsOpen )
                {
                    _Logger.LogError( "Canal RabbitMQ não está aberto." );
                    return;
                }

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
                _Logger.LogError( ex, "Erro ao garantir a inicialização do RabbitMQ." );
                Console.WriteLine( "RabbitMq initialization failed." );

                await DisposeAsync( );

                throw new InvalidOperationException( $"Erro ao inicializar fila {queueName}", ex );
            }

            _semaphore.Release( );
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
                    _Logger.LogError( ex, $"Erro ao processar mensagem: {ex.Message}" );
                    await _Channel.BasicNackAsync( ea.DeliveryTag, false, true );
                }

                stopwatch.Stop( );

                _Logger.LogInformation( $"Tempo total do processamento do pedido: {stopwatch.ElapsedMilliseconds} ms" );

                countMessage++;

                if ( countMessage % intervalDelay == 0 )
                {
                    _Logger.LogInformation( $"Processadas {countMessage} mensagens. Aguardando {intervalDelay} ms antes de continuar..." );
                    await Task.Delay( 1000 );
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
                await Task.Delay( 10000, stoppingToken );
            }
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

        private async Task MonitorConnectionAsync( CancellationToken stoppingToken )
        {
            while ( !stoppingToken.IsCancellationRequested )
            {
                if ( _Connection == null || !_Connection.IsOpen || _Channel == null || !_Channel.IsOpen )
                {
                    _Logger.LogWarning( "Conexão ou canal RabbitMQ não está aberto. Tentando reconectar..." );
                    await Task.Delay( 30000, stoppingToken );
                    await InitAsync( );
                }

                await Task.Delay( 5000, stoppingToken );
            }
        }

        public async ValueTask DisposeAsync( )
        {
            _Logger.LogInformation( "Fechando conexão RabbitMQ..." );
            await CloseConnectionAsync( );
        }
    }
}
