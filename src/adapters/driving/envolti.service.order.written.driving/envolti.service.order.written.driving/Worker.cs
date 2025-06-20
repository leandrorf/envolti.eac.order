using envolti.lib.order.application.Order.Responses;
using envolti.lib.order.domain.Order.Adapters;
using envolti.lib.order.domain.Order.Enums;
using envolti.lib.order.domain.Order.Exceptions;
using envolti.lib.order.domain.Order.Ports;
using System.Diagnostics;
using System.Text.Json;

namespace envolti.service.order.driving
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _Logger;
        private readonly IOrderQueuesAdapter _OrderQueueAdapter;
        private readonly IServiceProvider _ServiceProvider;
        private readonly IOrderRedisAdapter _OrderRedisAdapter;

        public Worker(
            ILogger<Worker> logger,
            IOrderQueuesAdapter orderQueueAdapter,
            IServiceProvider serviceProvider,
            IOrderRedisAdapter orderRedisAdapter 
            )
        {
            _Logger = logger;
            _OrderQueueAdapter = orderQueueAdapter;
            _ServiceProvider = serviceProvider;
            _OrderRedisAdapter = orderRedisAdapter;
        }

        protected override async Task ExecuteAsync( CancellationToken stoppingToken )
        {
            Stopwatch stopwatch = new Stopwatch( );

            _Logger.LogInformation( "Worker iniciado em: {time}", DateTimeOffset.Now );

            await using var scope = _ServiceProvider.CreateAsyncScope( );
            var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>( );

            while ( !stoppingToken.IsCancellationRequested )
            {
                if ( _Logger.IsEnabled( LogLevel.Information ) )
                {
                    _Logger.LogInformation( "Worker running at: {time}", DateTimeOffset.Now );
                }

                await _OrderQueueAdapter.ConsumerOrderAsync( "order_queue", stoppingToken,
                    async ( order ) =>
                    {
                        try
                        {
                            stopwatch.Restart( );

                            var orderEntity = OrderQueuesAdapter.MapToEntity( order );
                            await orderEntity.Save( orderRepository );
                            await _OrderRedisAdapter.PublishOrderAsync( "orders", orderEntity.MapEntityToDto( ) );

                            stopwatch.Stop( );

                            _Logger.LogInformation( $"Pedido {order.OrderIdExternal} processado com sucesso. Tempo gasto: {stopwatch.ElapsedMilliseconds} ms" );
                        }
                        catch ( TheOrderNumberCannotBeRepeatedException )
                        {
                            var result = new OrderResponse
                            {
                                Data = null,
                                Success = false,
                                Message = "The order number cannot be repeated.",
                                ErrorCode = ErrorCodesResponseEnum.THE_ORDER_NUMBER_CANNOT_BE_REPEATED
                            };

                            var jsonResult = JsonSerializer.Serialize( result, new JsonSerializerOptions { WriteIndented = true } );
                            _Logger.LogError( $"Pedido duplicado: {jsonResult}" );
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
