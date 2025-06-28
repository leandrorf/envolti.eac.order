using envolti.lib.order.domain.Order.Entities;

namespace envolti.lib.order.domain.Order.Dtos
{
    public class OrderRequestDto
    {
        public required int OrderIdExternal { get; set; }
        public required List<ProductRequestDto> Products { get; set; } = new( );

        public static OrderRequestDto MapToDto( OrderEntity order )
        {
            return new OrderRequestDto
            {
                OrderIdExternal = order.OrderIdExternal,
                Products = order.Products.Select( p => new ProductRequestDto
                {
                    ProductIdExternal = p.ProductIdExternal,
                    Name = p.Name,
                    Price = p.Price
                } ).ToList( )
            };
        }
    }
}
