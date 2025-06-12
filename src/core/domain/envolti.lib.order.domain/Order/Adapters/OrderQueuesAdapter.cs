using envolti.lib.order.domain.Order.Dtos;
using envolti.lib.order.domain.Order.Entities;
using envolti.lib.order.domain.Order.Enums;
using envolti.lib.order.domain.Order.Exceptions;
using envolti.lib.order.domain.Order.Ports;
using System.Runtime.CompilerServices;

namespace envolti.lib.order.domain.Order.Adapters
{
    public class OrderQueuesAdapter : OrderRequestDto
    {
        private async Task ValidateState( IOrderQueuesAdapter orderAdapter )
        {
            if ( await orderAdapter.Exists( "order_queue", OrderIdExternal ) )
            {
                throw new TheOrderNumberCannotBeRepeatedException( );
            }
        }

        public static OrderResponseDto MapToDto( OrderQueuesAdapter dto )
        {
            return new OrderResponseDto
            {
                OrderIdExternal = dto.OrderIdExternal,
                Products = dto.Products.Select( x => new OrderResponseDto.Produtos
                {
                    Id = x.Id,
                    ProductIdExternal = x.ProductIdExternal,
                    Name = x.Name,
                    Price = x.Price
                } ).ToList( )
            };
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

        public async Task Save( IOrderQueuesAdapter orderAdapter )
        {
            await ValidateState( orderAdapter );

            if ( Id == 0 )
            {
                var resp = await orderAdapter.PublishOrderAsync( this, "order_queue" );
                Id = resp.Id;
            }
        }
    }
}
