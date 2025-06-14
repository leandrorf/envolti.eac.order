using envolti.lib.order.domain.Order.Adapters;
using envolti.lib.order.domain.Order.Ports;

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
            IOrderRedisAdapter orderRedisAdapter )
        {
            _Logger = logger;
            _OrderQueueAdapter = orderQueueAdapter;
            _ServiceProvider = serviceProvider;
            _OrderRedisAdapter = orderRedisAdapter;
        }

        protected override async Task ExecuteAsync( CancellationToken stoppingToken )
        {
            await using var scope = _ServiceProvider.CreateAsyncScope( );
            var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>( );

            while ( !stoppingToken.IsCancellationRequested )
            {
                if ( _Logger.IsEnabled( LogLevel.Information ) )
                {
                    _Logger.LogInformation( "Worker running at: {time}", DateTimeOffset.Now );
                }

                _ = Task.Run( async ( ) =>
                {
                    await _OrderQueueAdapter.ConsumerOrderAsync(
                        "order_queue",
                        async ( order ) =>
                        {
                            var orderEntity = OrderQueuesAdapter.MapToEntity( order );
                            await orderEntity.Save( orderRepository ).ConfigureAwait( false );
                            await _OrderRedisAdapter.PublishOrderAsync( "orders", orderEntity.MapEntityToDto( ) ).ConfigureAwait( false );
                        },
                        stoppingToken
                    ).ConfigureAwait( false );
                }, stoppingToken );

                await Task.Delay( 300, stoppingToken ).ConfigureAwait( false );
            }
        }

        public override async Task<Task> StopAsync( CancellationToken stoppingToken )
        {
            await _OrderQueueAdapter.CloseConnectionAsync( ).ConfigureAwait( false );
            return Task.CompletedTask;
        }
    }
}
