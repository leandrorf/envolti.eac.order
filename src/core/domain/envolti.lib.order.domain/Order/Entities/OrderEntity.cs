﻿using envolti.lib.order.domain.Order.Dtos;
using envolti.lib.order.domain.Order.Enums;
using envolti.lib.order.domain.Order.Exceptions;
using envolti.lib.order.domain.Order.Ports;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace envolti.lib.order.domain.Order.Entities
{
    public class OrderEntity
    {
        [BsonId]
        [BsonRepresentation( BsonType.ObjectId )]
        public string? Id { get; set; }
        public int OrderIdExternal { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedIn { get; set; }
        public StatusEnum Status { get; set; }
        public List<ProductEntity> Products { get; set; } = new( );

        private async Task ValidateState( IOrderRepository order )
        {
            if ( await order.OrderExistsAsync( OrderIdExternal ) )
            {
                throw new TheOrderNumberCannotBeRepeatedException( );
            }
        }

        private void ProcessOrders( )
        {
            ProcessedIn = DateTime.Now;
            Status = StatusEnum.Processed;

            if ( Products.Any( ) )
            {
                TotalPrice = Products.Sum( p => p.Price );

                foreach ( var produto in Products )
                {
                    produto.Id ??= ObjectId.GenerateNewId( ).ToString( );
                    produto.CreatedAt = DateTime.Now;
                }
            }
            else
            {
                TotalPrice = 0;
            }
        }

        public OrderResponseDto MapEntityToDto( )
        {
            return new OrderResponseDto
            {
                Id = Id,
                OrderIdExternal = OrderIdExternal,
                TotalPrice = TotalPrice,
                CreatedAt = CreatedAt,
                ProcessedIn = ProcessedIn ?? DateTime.MinValue,
                Status = Status,
                Products = Products.Select( p => new ProductResponseDto
                {
                    Id = p.Id,
                    ProductIdExternal = p.ProductIdExternal,
                    Name = p.Name,
                    Price = p.Price
                } ).ToList( )
            };
        }

        public async Task Save( IOrderRepository order )
        {
            await ValidateState( order );

            if ( Id == null )
            {
                ProcessOrders( );

                var resp = await order.CreateOrderAsync( this );
                Id = resp.Id;
            }
        }
    }
}
