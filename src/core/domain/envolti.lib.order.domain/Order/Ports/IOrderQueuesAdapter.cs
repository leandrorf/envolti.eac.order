
using envolti.lib.order.domain.Order.Dtos;

namespace envolti.lib.order.domain.Order.Ports
{
    public interface IOrderQueuesAdapter
    {
        Task<bool> Exists( string queueName, int correlationId );
        Task ConsumerOrderAsync( string queueName, Func<OrderRequestDto, Task> processOrderCallback, CancellationToken stoppingToken );
        Task<OrderRequestDto> PublishOrderAsync( OrderRequestDto order, string queueName );
        Task CloseConnectionAsync( );
    }
}
