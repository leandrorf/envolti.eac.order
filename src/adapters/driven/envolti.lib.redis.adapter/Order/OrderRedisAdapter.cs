using envolti.lib.order.domain.Order.Ports;
using envolti.lib.order.domain.Order.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace envolti.lib.redis.adapter.Order
{
    public class OrderRedisAdapter : IOrderRedisAdapter, IAsyncDisposable
    {
        private IDatabase _Redis = null!;
        private readonly Lazy<Task> _initTask;
        private readonly IOptions<RedisSettings> _Settings;
        private readonly ILogger<OrderRedisAdapter> _Logger;

        public OrderRedisAdapter( ILogger<OrderRedisAdapter> logger, IOptions<RedisSettings> settings )
        {
            _initTask = new Lazy<Task>( InitAsync );
            Task.Run( ( ) => MonitorConnectionAsync( CancellationToken.None ) );
            _Logger = logger ?? throw new ArgumentNullException( nameof( logger ) );
            _Settings = settings ?? throw new ArgumentNullException( nameof( settings ) );
        }

        public async Task InitAsync( )
        {
            if ( _Redis != null )
            {
                return;
            }

            int retryCount = 5;
            int delayMilliseconds = 2000;

            for ( int i = 0; i < retryCount; i++ )
            {
                try
                {
                    _Logger.LogInformation( "Conectando ao Redis. Tentativa {Attempt}", i + 1 );
                    _Logger.LogInformation( "Host: {Host}", _Settings.Value.Host );

                    var redis = await ConnectionMultiplexer.ConnectAsync( _Settings.Value.Host );
                    _Redis = redis.GetDatabase( );

                    await CreateKeyAsync( );
                    return;
                }
                catch ( Exception ex )
                {
                    _Logger.LogError( ex, "Erro ao conectar ao Redis. Tentativa {Attempt}", i + 1 );
                    await Task.Delay( delayMilliseconds );
                }
            }

            _Logger.LogError( "Falha ao conectar ao Redis após múltiplas tentativas." );
            throw new Exception( "Falha ao conectar ao Redis após múltiplas tentativas." );
        }

        private async Task EnsureInitializedAsync( )
        {
            try
            {
                await _initTask.Value;
            }
            catch ( Exception ex )
            {
                _Logger.LogError( ex, "Erro ao inicializar Redis: {Message}", ex.Message );
                throw new Exception( ex.Message );
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
            await EnsureInitializedAsync( );

            if ( _Redis?.Multiplexer == null || !_Redis.Multiplexer.IsConnected )
            {
                _Logger.LogWarning( "Redis não está conectado. Retornando valor padrão." );
                return default;
            }

            var result = await _Redis.ExecuteAsync( command, key, query );
            var jsonString = result.ToString( );

            return string.IsNullOrEmpty( jsonString )
                ? default
                : JsonConvert.DeserializeObject<List<T>>( jsonString ).FirstOrDefault( );

        }

        public async Task<T> ConsumerOrderAllAsync<T>( string key, string command, string query )
        {
            await EnsureInitializedAsync( );

            var result = await _Redis.ExecuteAsync( command, key, query );
            var jsonString = result.ToString( );

            return string.IsNullOrEmpty( jsonString )
                ? default
                : JsonConvert.DeserializeObject<T>( jsonString );
        }

        public async Task<bool> PublishOrderAsync<T>( string key, T value )
        {
            await EnsureInitializedAsync( );

            string json = JsonConvert.SerializeObject( value );

            var result = await _Redis.ExecuteAsync( "JSON.ARRAPPEND", key, "$.items", json );

            if ( result.IsNull )
            {
                return false;
            }

            if ( result.Resp2Type == ResultType.Integer )
            {
                return ( int )result > 0;
            }

            if ( result.Resp2Type == ResultType.Array )
            {
                var values = result; // Converte para array de RedisResult
                return values.Length > 0 && Convert.ToInt32( values[ 0 ] ) > 0;
            }

            _Logger.LogError( $"Tipo inesperado no retorno do Redis: {result.Resp2Type}" );
            return false;
        }

        public async Task CloseConnectionAsync( )
        {
            if ( _Redis?.Multiplexer != null && _Redis.Multiplexer.IsConnected )
            {
                try
                {
                    await _Redis.Multiplexer.CloseAsync( );
                }
                catch ( Exception ex )
                {
                    _Logger.LogError( ex, "Erro ao fechar conexão com Redis: {Message}", ex.Message );
                }
            }

        }

        private async Task MonitorConnectionAsync( CancellationToken stoppingToken )
        {
            while ( !stoppingToken.IsCancellationRequested )
            {
                if ( _Redis?.Multiplexer == null || !_Redis.Multiplexer.IsConnected )
                {
                    _Logger.LogWarning( "Conexão com Redis perdida. Tentando reconectar..." );
                    await InitAsync( );
                }

                await Task.Delay( 5000, stoppingToken );
            }
        }

        public async ValueTask DisposeAsync( )
        {
            await CloseConnectionAsync( );
        }
    }
}
