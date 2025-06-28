using envolti.lib.order.domain.Order.Dtos;
using envolti.lib.order.domain.Order.Entities;
using envolti.lib.order.domain.Order.Ports;

namespace envolti.lib.order.domain.Order.Adapters
{
    public class OrderCacheAdapter : OrderRequestDto
    {
        public static OrderCacheAdapter MapToDto( OrderRequestDto dto )
        {
            return new OrderCacheAdapter
            {
                OrderIdExternal = dto.OrderIdExternal,
                Products = dto.Products.Select( p => new ProductRequestDto
                {
                    ProductIdExternal = p.ProductIdExternal,
                    Name = p.Name,
                    Price = p.Price
                } ).ToList( )
            };
        }

        public static OrderEntity MapToEntity( OrderRequestDto dto )
        {
            return new OrderEntity
            {
                OrderIdExternal = dto.OrderIdExternal,
                Products = dto.Products.Select( p => new ProductEntity
                {
                    ProductIdExternal = p.ProductIdExternal,
                    Name = p.Name,
                    Price = p.Price
                } ).ToList( )
            };
        }

        public async Task Save( IOrderCacheAdapter order )
        {
            await order.PublishOrderAsync( order );
        }
    }
}
