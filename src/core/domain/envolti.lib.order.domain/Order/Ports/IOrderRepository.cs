using envolti.lib.order.domain.Order.Dtos;
using envolti.lib.order.domain.Order.Entities;
using envolti.lib.order.domain.Order.Enums;

namespace envolti.lib.order.domain.Order.Ports
{
    public interface IOrderRepository
    {
        Task<bool> OrderExistsAsync( int id );
        Task<PagedResult<OrderEntity>> GetAllAsync( int pageNumber, int pageSize );
        Task<OrderEntity?> GetOrderByIdAsync( int id );
        Task<OrderEntity> CreateOrderAsync( OrderEntity order );
    }
}
