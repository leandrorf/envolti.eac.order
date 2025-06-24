using envolti.lib.order.domain.Order.Dtos;
using envolti.lib.order.domain.Order.Enums;

namespace envolti.lib.order.domain.Order.Ports
{
    public interface IOrderCacheAdapter
    {
        Task<T> ConsumerOrderByIdAsync<T>( string property, int id );
        Task<PagedResult<T>> ConsumerOrderAllAsync<T>( int pageNumber, int pageSize );
        Task<PagedResult<T>> GetOrdersByStatusAsync<T>( string property, StatusEnum status, int pageNumber, int pageSize );
        Task<bool> PublishOrderAsync<T>( T value );
        Task InitAsync( );
        Task CloseConnectionAsync( );
        Task<int> ListLengthAsync( );
    }
}
