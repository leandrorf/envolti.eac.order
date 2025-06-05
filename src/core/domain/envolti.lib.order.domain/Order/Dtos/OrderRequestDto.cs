using envolti.lib.order.domain.Order.Entities;

namespace envolti.lib.order.domain.Order.Dtos
{
    public class OrderRequestDto
    {
        public int Id { get; set; } = 0;
        public required int OrderIdExternal { get; set; }
        public required List<Produtos> Products { get; set; } = new( );

        public class Produtos
        {
            public int Id = 0;
            public required int ProductIdExternal { get; set; }
            public required string Name { get; set; }
            public required decimal Price { get; set; }
        }

        public static OrderRequestDto MapToDto( OrderEntity order )
        {
            return new OrderRequestDto
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
