using envolti.lib.order.domain.Order.Entities;
using envolti.lib.order.domain.Order.Enums;

namespace envolti.lib.order.domain.Order.Ports
{
    public interface IOrderRepository
    {
        Task<bool> OrderExistsAsync( int id );
        Task<IEnumerable<OrderEntity>> GetAllAsync( int pageNumber, int pageSize );
        Task<OrderEntity?> GetOrderByIdAsync( int id );
        Task<OrderEntity> CreateOrderAsync( OrderEntity order );
        Task<IEnumerable<OrderEntity>> GetOrdersByStatusAsync( StatusEnum status, int pageNumber, int pageSize );
    }
}
