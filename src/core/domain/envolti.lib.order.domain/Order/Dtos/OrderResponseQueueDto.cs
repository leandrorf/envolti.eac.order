using envolti.lib.order.domain.Order.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace envolti.lib.order.domain.Order.Dtos
{
    public class OrderResponseQueueDto
    {
        public required int OrderIdExternal { get; set; }
        public required List<Produtos> Products { get; set; } = new( );

        public class Produtos
        {
            public required int ProductIdExternal { get; set; }
            public required string Name { get; set; }
            public required decimal Price { get; set; }
        }

        public static OrderResponseQueueDto MapToDto( OrderRequestDto order )
        {
            return new OrderResponseQueueDto
            {
                OrderIdExternal = order.OrderIdExternal,
                Products = order.Products.Select( p => new Produtos
                {
                    ProductIdExternal = p.ProductIdExternal,
                    Name = p.Name,
                    Price = p.Price
                } ).ToList( )
            };
        }
    }
}
