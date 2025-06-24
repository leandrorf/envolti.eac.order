using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace envolti.lib.order.domain.Order.Entities
{
    public class ProductEntity
    {
        [BsonId]
        [BsonRepresentation( BsonType.ObjectId )]
        public string? Id { get; set; }
        public int ProductIdExternal { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
