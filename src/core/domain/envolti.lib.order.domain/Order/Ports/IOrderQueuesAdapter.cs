
using envolti.lib.order.domain.Order.Dtos;

namespace envolti.lib.order.domain.Order.Ports
{
    public interface IOrderQueuesAdapter
    {
        Task<bool> Exists( string queueName, int correlationId );
        Task<uint> Unprocessed( string queueName );
        Task ConsumerOrderAsync( string queueName, CancellationToken stoppingToken, Func<OrderRequestDto, Task> processOrderCallback );
        Task<OrderRequestDto> PublishOrderAsync( OrderRequestDto order, string queueName );
        Task CloseConnectionAsync( );
    }
}
