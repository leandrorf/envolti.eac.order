using envolti.lib.order.domain.Order.Adapters;
using envolti.lib.order.domain.Order.Ports;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace envolti.lib.order.application.Order.Services
{
    public class OrderService : BackgroundService
    {
        private readonly IOrderQueuesAdapter _OrderQueueAdapter;
        private readonly IServiceProvider _ServiceProvider;
        private readonly IOrderRedisAdapter _OrderRedisAdapter;

        public OrderService(
            IOrderQueuesAdapter orderAdapter,
            IServiceProvider serviceProvider,
            IOrderRedisAdapter orderRedisAdapter )
        {
            _OrderQueueAdapter = orderAdapter;
            _ServiceProvider = serviceProvider;
            _OrderRedisAdapter = orderRedisAdapter;
        }

        protected override async Task ExecuteAsync( CancellationToken stoppingToken )
        {
            using ( var scope = _ServiceProvider.CreateAsyncScope( ) ) // Criar escopo para resolver Scoped services
            {
                var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>( );

                while ( !stoppingToken.IsCancellationRequested )
                {
                    await _OrderQueueAdapter.ConsumerOrderAsync( "order_queue",
                        async ( order ) =>
                        {
                            var orderEntity = OrderQueuesAdapter.MapToEntity( order );
                            await orderEntity.Save( orderRepository );
                            await _OrderRedisAdapter.PublishOrderAsync( "orders", orderEntity.MapEntityToDto( ) );
                        }
                        , stoppingToken );
                }
            }
        }

        public override async Task<Task> StopAsync( CancellationToken stoppingToken )
        {
            await _OrderQueueAdapter.CloseConnectionAsync( );
            return Task.CompletedTask;
        }
    }
}