﻿namespace envolti.lib.order.domain.Order.Ports
{
    public interface IOrderRedisAdapter
    {
        Task<T> ConsumerOrderByIdAsync<T>( string key, string commnad, string query );
        Task<T> ConsumerOrderAllAsync<T>( string key, string command, string query );
        Task<bool> PublishOrderAsync<T>( string key, T value );
        Task InitAsync( );
        Task CloseConnectionAsync( );
        Task CreateKeyAsync( );
    }
}
