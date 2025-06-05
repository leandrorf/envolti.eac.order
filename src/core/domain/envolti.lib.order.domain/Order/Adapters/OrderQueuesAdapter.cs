using envolti.lib.order.domain.Order.Dtos;
using envolti.lib.order.domain.Order.Entities;
using envolti.lib.order.domain.Order.Enums;
using envolti.lib.order.domain.Order.Ports;

namespace envolti.lib.order.domain.Order.Adapters
{
    public class OrderQueuesAdapter : OrderRequestDto
    {
        private void ValidateState( )
        {

        }

        public static OrderQueuesAdapter MapToAdapter( OrderRequestDto dto )
        {
            return new OrderQueuesAdapter
            {
                OrderIdExternal = dto.OrderIdExternal,
                Products = dto.Products.Select( p => new OrderRequestDto.Produtos
                {
                    Id = 0,
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
                Status = StatusEnum.Created,
                CreatedAt = DateTime.Now,
                Products = dto.Products.Select( p => new ProductEntity
                {
                    ProductIdExternal = p.ProductIdExternal,
                    Name = p.Name,
                    Price = p.Price,
                    CreatedAt = DateTime.Now
                } ).ToList( )
            };
        }

        public async Task Save( IOrderQueuesAdapter order )
        {
            ValidateState( );

            if ( Id == 0 )
            {
                var resp = await order.PublishOrderAsync( this, "order_queue" );
                Id = resp.Id;
            }
        }
    }
}
