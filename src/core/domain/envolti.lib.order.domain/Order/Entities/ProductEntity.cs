namespace envolti.lib.order.domain.Order.Entities
{
    public class ProductEntity
    {
        public int Id { get; set; }
        public int OrderId { get; set; } // Foreign key to OrderEntity
        public int ProductIdExternal { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
