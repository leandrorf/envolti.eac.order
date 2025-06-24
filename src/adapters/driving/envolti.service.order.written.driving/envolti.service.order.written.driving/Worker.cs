using envolti.lib.order.application.Order.Responses;
using envolti.lib.order.domain.Order.Adapters;
using envolti.lib.order.domain.Order.Enums;
using envolti.lib.order.domain.Order.Exceptions;
using envolti.lib.order.domain.Order.Ports;
using envolti.lib.order.domain.Order.Settings;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text.Json;

namespace envolti.service.order.driving
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _Logger;
        private readonly IOrderQueuesAdapter _OrderQueueAdapter;
        private readonly IServiceProvider _ServiceProvider;
        private readonly IOrderCacheAdapter _OrderRedisAdapter;
        private readonly IOptions<RabbitMqSettings> _Settings;

        public Worker(
            ILogger<Worker> logger,
            IOrderQueuesAdapter orderQueueAdapter,
            IServiceProvider serviceProvider,
            IOrderCacheAdapter orderRedisAdapter,
            IOptions<RabbitMqSettings> settings )
        {
            _Logger = logger;
            _OrderQueueAdapter = orderQueueAdapter;
            _ServiceProvider = serviceProvider;
            _OrderRedisAdapter = orderRedisAdapter;
            _Settings = settings;
        }

        protected override async Task ExecuteAsync( CancellationToken stoppingToken )
        {
            Stopwatch stopwatch = new Stopwatch( );

            _Logger.LogInformation( "Worker iniciado em: {time}", DateTimeOffset.Now );

            await using var scope = _ServiceProvider.CreateAsyncScope( );
            var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>( );

            if ( _Settings?.Value?.Queue?.OrderQueue == null )
            {
                _Logger.LogError( "OrderQueue setting is not configured properly." );
                return;
            }

            while ( !stoppingToken.IsCancellationRequested )
            {
                if ( _Logger.IsEnabled( LogLevel.Information ) )
                {
                    _Logger.LogInformation( "Worker running at: {time}", DateTimeOffset.Now );
                }

                await _OrderQueueAdapter.ConsumerOrderAsync( _Settings.Value.Queue.OrderQueue, stoppingToken,
                    async ( order ) =>
                    {
                        try
                        {
                            stopwatch.Restart( );

                            _Logger.LogInformation( "Processando pedido: {order.OrderIdExternal}", order.OrderIdExternal );

                            var orderEntity = OrderQueuesAdapter.MapToEntity( order );
                            await orderEntity.Save( orderRepository );
                            await _OrderRedisAdapter.PublishOrderAsync( orderEntity.MapEntityToDto( ) );

                            stopwatch.Stop( );

                            _Logger.LogInformation( "Pedido {order.OrderIdExternal} processado com sucesso. Tempo gasto: {stopwatch.ElapsedMilliseconds} ms", order.OrderIdExternal, stopwatch.ElapsedMilliseconds );
                        }
                        catch ( TheOrderNumberCannotBeRepeatedException )
                        {
                            var result = new OrderListResponse
                            {
                                Data = null!,
                                Success = false,
                                Message = "The order number cannot be repeated.",
                                ErrorCode = ErrorCodesResponseEnum.THE_ORDER_NUMBER_CANNOT_BE_REPEATED
                            };

                            var jsonResult = JsonSerializer.Serialize( result, new JsonSerializerOptions { WriteIndented = true } );
                            _Logger.LogError( "Pedido duplicado: {jsonResult}", jsonResult );
                        }
                        catch ( Exception ex )
                        {
                            _Logger.LogError( ex, "Erro ao processar o pedido." );
                        }
                    }
                );

                await Task.Delay( 1000, stoppingToken );
            }
        }

        public override async Task<Task> StopAsync( CancellationToken stoppingToken )
        {
            await _OrderQueueAdapter.CloseConnectionAsync( );
            return Task.CompletedTask;
        }
    }
}
