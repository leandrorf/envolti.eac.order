using envolti.lib.order.domain.Order.Entities;
using envolti.lib.order.domain.Order.Enums;
using envolti.lib.order.domain.Order.Ports;
using envolti.lib.order.domain.Order.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace envolti.lib.data.mongodb.Order
{
    public class OrderRepositoryMongoAdapter : IOrderRepository
    {
        private readonly IMongoCollection<OrderEntity> _collection;

        public OrderRepositoryMongoAdapter( IOptions<MongoSettings> settings )
        {
            var client = new MongoClient( settings.Value.ConnectionString );
            var database = client.GetDatabase( settings.Value.DatabaseName );
            _collection = database.GetCollection<OrderEntity>( "orders" );
        }

        public async Task<OrderEntity> CreateOrderAsync( OrderEntity order )
        {
            await _collection.InsertOneAsync( order );
            return order;
        }

        public async Task<IEnumerable<OrderEntity>> GetAllAsync( )
        {
            var result = await _collection.FindAsync( FilterDefinition<OrderEntity>.Empty );
            return result.ToList( );
        }

        public async Task<OrderEntity?> GetOrderByIdAsync( int id )
        {
            var filter = Builders<OrderEntity>.Filter.Eq( o => o.OrderIdExternal, id );
            var result = await _collection.FindAsync( filter );
            return await result.FirstOrDefaultAsync( );
        }

        public async Task<IEnumerable<OrderEntity>> GetOrdersByStatusAsync( StatusEnum status )
        {
            var filter = Builders<OrderEntity>.Filter.Eq( o => o.Status, status );
            var result = await _collection.FindAsync( filter );
            return result.ToList( );
        }

        public async Task<bool> OrderExistsAsync( int id )
        {
            var filter = Builders<OrderEntity>.Filter.Eq( o => o.OrderIdExternal, id );
            var count = await _collection.CountDocumentsAsync( filter );
            return count > 0;
        }
    }
}

