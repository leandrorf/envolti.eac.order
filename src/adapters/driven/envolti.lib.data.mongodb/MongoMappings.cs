using envolti.lib.order.domain.Order.Entities;
using MongoDB.Bson.Serialization;

namespace envolti.lib.data.mongodb
{
    public static class MongoMappings
    {
        public static void RegisterClassMaps( )
        {
            if ( !BsonClassMap.IsClassMapRegistered( typeof( ProductEntity ) ) )
            {
                BsonClassMap.RegisterClassMap<ProductEntity>( cm =>
                {
                    cm.AutoMap( );
                    cm.MapIdMember( p => p.Id )
                        .SetIdGenerator( MongoDB.Bson.Serialization.IdGenerators.StringObjectIdGenerator.Instance );
                } );
            }
        }

    }
}
