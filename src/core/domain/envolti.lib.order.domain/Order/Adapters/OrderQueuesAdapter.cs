﻿using envolti.lib.order.domain.Order.Dtos;
using envolti.lib.order.domain.Order.Entities;
using envolti.lib.order.domain.Order.Enums;
using envolti.lib.order.domain.Order.Exceptions;
using envolti.lib.order.domain.Order.Ports;

namespace envolti.lib.order.domain.Order.Adapters
{
    public class OrderQueuesAdapter : OrderRequestDto
    {
        private async Task ValidateState( IOrderQueuesAdapter orderAdapter, string queueName )
        {
            if ( await orderAdapter.Exists( queueName, OrderIdExternal ) )
            {
                throw new TheOrderNumberCannotBeRepeatedException( );
            }
        }

        public static OrderQueuesAdapter MapToAdapter( OrderRequestDto dto )
        {
            return new OrderQueuesAdapter
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

        public OrderResponseQueueDto MapAdapterToDto( )
        {
            return new OrderResponseQueueDto
            {
                OrderIdExternal = OrderIdExternal,
                Products = Products.Select( x => new ProductResponseDto
                {
                    ProductIdExternal = x.ProductIdExternal,
                    Name = x.Name,
                    Price = x.Price
                } ).ToList( )
            };
        }

        public async Task Save( IOrderQueuesAdapter orderAdapter, string queueName )
        {
            await ValidateState( orderAdapter, queueName );
            var resp = await orderAdapter.PublishOrderAsync( this, queueName );
        }
    }
}
