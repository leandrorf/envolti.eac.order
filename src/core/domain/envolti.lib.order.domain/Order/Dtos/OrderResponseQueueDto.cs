namespace envolti.lib.order.domain.Order.Dtos
{
    public class OrderResponseQueueDto
    {
        public required int OrderIdExternal { get; set; }
        public required List<ProductResponseDto> Products { get; set; } = new( );

        public static OrderResponseQueueDto MapToDto( OrderRequestDto order )
        {
            return new OrderResponseQueueDto
            {
                OrderIdExternal = order.OrderIdExternal,
                Products = order.Products.Select( p => new ProductResponseDto
                {
                    ProductIdExternal = p.ProductIdExternal,
                    Name = p.Name,
                    Price = p.Price
                } ).ToList( )
            };
        }
    }
}
