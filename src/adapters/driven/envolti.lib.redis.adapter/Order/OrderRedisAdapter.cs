using envolti.lib.order.domain.Order.Ports;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace envolti.lib.redis.adapter.Order
{
    public class OrderRedisAdapter : IOrderRedisAdapter
    {
        private static IDatabase _Redis;

        public async Task InitAsync( )
        {
            if ( _Redis == null )
            {
                var redis = await ConnectionMultiplexer.ConnectAsync( "localhost" );
                _Redis = redis.GetDatabase( );

                await CreateKeyAsync( );
            }
        }

        public async Task CreateKeyAsync( )
        {
            if ( !await _Redis.KeyExistsAsync( "orders" ) )
            {
                await _Redis.ExecuteAsync( "JSON.SET", "orders", "$", "{ \"items\": [] }" );
            }
        }

        // Explicit implementation to resolve CS0425 and CS8613
        async Task<T> IOrderRedisAdapter.ConsumerOrderByIdAsync<T>( string key, string command, string query )
        {
            await InitAsync( );

            var result = await _Redis.ExecuteAsync( command, key, query );
            var jsonString = result.ToString( );

            return string.IsNullOrEmpty( jsonString )
                ? default
                : JsonConvert.DeserializeObject<List<T>>( jsonString ).FirstOrDefault( );
        }

        public async Task<T> ConsumerOrderAllAsync<T>( string key, string command, string query )
        {
            await InitAsync( );

            var result = await _Redis.ExecuteAsync( command, key, query );
            var jsonString = result.ToString( );

            return string.IsNullOrEmpty( jsonString )
                ? default
                : JsonConvert.DeserializeObject<T>( jsonString );
        }

        public async Task PublishOrderAsync<T>( string key, T value )
        {
            await InitAsync( );
            string json = JsonConvert.SerializeObject( value );
            await _Redis.ExecuteAsync( "JSON.ARRAPPEND", key, "$.items", json );
        }

        public Task CloseConnectionAsync( )
        {
            throw new NotImplementedException( );
        }
    }
}
