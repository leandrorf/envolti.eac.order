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

        public async Task<IEnumerable<OrderEntity>> GetAllAsync( int pageNumber, int pageSize )
        {
            return await _collection.Find( FilterDefinition<OrderEntity>.Empty )
                .Skip( ( pageNumber - 1 ) * pageSize )
                .Limit( pageSize )
                .ToListAsync( );
        }

        public async Task<OrderEntity?> GetOrderByIdAsync( int id )
        {
            var filter = Builders<OrderEntity>.Filter.Eq( o => o.OrderIdExternal, id );
            var result = await _collection.FindAsync( filter );
            return await result.FirstOrDefaultAsync( );
        }

        public async Task<IEnumerable<OrderEntity>> GetOrdersByStatusAsync( StatusEnum status, int pageNumber, int pageSize )
        {
            var filter = Builders<OrderEntity>.Filter.Eq( o => o.Status, status );

            return await _collection.Find( filter )
                .Skip( ( pageNumber - 1 ) * pageSize )
                .Limit( pageSize )
                .ToListAsync( );
        }

        public async Task<bool> OrderExistsAsync( int id )
        {
            var filter = Builders<OrderEntity>.Filter.Eq( o => o.OrderIdExternal, id );
            var count = await _collection.CountDocumentsAsync( filter );
            return count > 0;
        }
    }
}

