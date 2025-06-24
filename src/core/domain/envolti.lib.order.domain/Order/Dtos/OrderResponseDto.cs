using envolti.lib.order.domain.Order.Entities;
using envolti.lib.order.domain.Order.Enums;

namespace envolti.lib.order.domain.Order.Dtos
{
    public class OrderResponseDto
    {
        public string? Id { get; set; }
        public required int OrderIdExternal { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ProcessedIn { get; set; }
        public StatusEnum Status { get; set; }
        public required List<Produtos> Products { get; set; } = new( );

        public class Produtos
        {
            public string? Id { get; set; }
            public required int ProductIdExternal { get; set; }
            public required string Name { get; set; }
            public required decimal Price { get; set; }
        }

        public static OrderResponseDto MapToDto( OrderEntity order )
        {
            return new OrderResponseDto
            {
                Id = order.Id,
                OrderIdExternal = order.OrderIdExternal,
                Products = order.Products.Select( p => new Produtos
                {
                    Id = p.Id,
                    ProductIdExternal = p.ProductIdExternal,
                    Name = p.Name,
                    Price = p.Price
                } ).ToList( )
            };
        }
    }
}
