using envolti.lib.order.domain.Order.Dtos;
using envolti.lib.order.domain.Order.Enums;
using envolti.lib.order.domain.Order.Exceptions;
using envolti.lib.order.domain.Order.Ports;
using envolti.lib.order.domain.Order.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NRedisStack.RedisStackCommands;
using StackExchange.Redis;

namespace envolti.lib.redis.adapter.Order
{
    public class OrderRedisAdapter : IOrderCacheAdapter, IAsyncDisposable
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

                    var host = _Settings.Value.Host ?? throw new ArgumentNullException( nameof( _Settings.Value.Host ), "Redis host configuration is null." );
                    var redis = await ConnectionMultiplexer.ConnectAsync( host );
                    _Redis = redis.GetDatabase( );

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

        public async Task<T> ConsumerOrderByIdAsync<T>( string property, int id )
        {
            try
            {
                await EnsureInitializedAsync( );

                var json = _Redis.JSON( );
                var fullKey = $"{_Settings.Value.DatabaseName}:{property}:{id}";
                var key = await _Redis.SortedSetRangeByRankAsync( fullKey );

                if ( key != null && key.Any( ) )
                {
                    var jsonStr = await json.GetAsync( key.FirstOrDefault( ).ToString( ) );
                    if ( jsonStr != null && !string.IsNullOrEmpty( jsonStr.ToString( ) ) )
                    {
                        var obj = JsonConvert.DeserializeObject<T>( jsonStr.ToString( ) );
                        if ( obj != null )
                        {
                            return obj;
                        }
                    }
                }
            }
            catch ( Exception ex )
            {
                throw new Exception( ex.Message, ex );
            }

            throw new RecordNotFoundException( );
        }

        public async Task<PagedResult<T>> ConsumerOrderAllAsync<T>( int pageNumber, int pageSize )
        {
            try
            {
                await EnsureInitializedAsync( );

                var fullKey = $"{_Settings.Value.DatabaseName}:sortedset";

                var total = ( int )await _Redis.SortedSetLengthAsync( fullKey );
                int start = ( pageNumber - 1 ) * pageSize;
                int end = start + pageSize - 1;

                var keys = await _Redis.SortedSetRangeByRankAsync( fullKey, start, end );

                var json = _Redis.JSON( );

                var results = new List<T>( );

                foreach ( var redisKey in keys )
                {
                    var jsonStr = await json.GetAsync( redisKey.ToString( ) );
                    if ( jsonStr != null && !string.IsNullOrEmpty( jsonStr.ToString( ) ) )
                    {
                        var obj = JsonConvert.DeserializeObject<T>( jsonStr.ToString( ) );
                        if ( obj != null )
                        {
                            results.Add( obj );
                        }
                    }
                }

                return new PagedResult<T>
                {
                    Items = results,
                    Total = total,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch ( Exception ex )
            {
                throw new Exception( ex.Message, ex );
            }
        }

        public async Task<bool> PublishOrderAsync<T>( T value )
        {
            try
            {
                await EnsureInitializedAsync( );

                var fullKey = $"{_Settings.Value.DatabaseName}:{Guid.NewGuid( ).ToString( )}";
                var json = _Redis.JSON( );
                var result = await json.SetAsync( fullKey, "$", value );

                if ( result )
                {
                    var sortedSetKey = $"{_Settings.Value.DatabaseName}:sortedset";
                    await _Redis.SortedSetAddAsync( sortedSetKey, fullKey, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds( ) );

                    var statusProp = value?.GetType( ).GetProperty( "Status" );
                    if ( statusProp != null )
                    {
                        var statusValue = statusProp.GetValue( value )?.ToString( );
                        if ( !string.IsNullOrEmpty( statusValue ) )
                        {
                            var statusKey = $"{_Settings.Value.DatabaseName}:status:{statusValue}";
                            await _Redis.SortedSetAddAsync( statusKey, fullKey, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds( ) );
                        }
                    }

                    var orderIdExternalProp = value?.GetType( ).GetProperty( "OrderIdExternal" );
                    if ( orderIdExternalProp != null )
                    {
                        var orderIdExternalValue = orderIdExternalProp.GetValue( value )?.ToString( );
                        if ( !string.IsNullOrEmpty( orderIdExternalValue ) )
                        {
                            var orderIdExternalKey = $"{_Settings.Value.DatabaseName}:orderidexternal:{orderIdExternalValue}";
                            await _Redis.SortedSetAddAsync( orderIdExternalKey, fullKey, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds( ) );
                        }
                    }

                    return true;
                }
            }
            catch ( Exception ex )
            {
                throw new Exception( ex.Message, ex );
            }

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

        public async Task<int> ListLengthAsync( )
        {
            try
            {
                var fullKey = $"{_Settings.Value.DatabaseName}:list";
                var total = await _Redis.ListLengthAsync( fullKey );
                if ( total > 0 )
                {
                    return ( int )total;
                }
            }
            catch ( Exception ex )
            {
                throw new Exception( ex.Message, ex );
            }

            return 0;
        }
    }
}
