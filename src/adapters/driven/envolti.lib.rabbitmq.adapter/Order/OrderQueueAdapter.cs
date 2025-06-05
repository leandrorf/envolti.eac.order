using envolti.lib.order.domain.Order.Dtos;
using envolti.lib.order.domain.Order.Ports;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;


namespace envolti.lib.rabbitmq.adapter.Order
{
    public class OrderQueueAdapter : IOrderQueuesAdapter
    {
        private static IConnection _Connection;
        private static IChannel _Channel;

        public async Task InitAsync( )
        {
            if ( _Connection == null )
            {
                var factory = new ConnectionFactory( ) { HostName = "localhost" };
                _Connection = await factory.CreateConnectionAsync( );
                _Channel = await _Connection.CreateChannelAsync( );
            }
        }

        public async Task ConsumerOrderAsync( string queueName, Func<OrderRequestDto, Task> processOrderCallback, CancellationToken stoppingToken )
        {
            await InitAsync( );

            //var tcs = new TaskCompletionSource<OrderRequestDto>( );

            await _Channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var consumer = new AsyncEventingBasicConsumer( _Channel );
            consumer.ReceivedAsync += async ( model, ea ) =>
            {
                var mensagem = Encoding.UTF8.GetString( ea.Body.ToArray( ) );

                var deserializedOrder = JsonConvert.DeserializeObject<OrderRequestDto>( mensagem );
                if ( deserializedOrder != null )
                {
                    await processOrderCallback( deserializedOrder );
                }
            };

            await _Channel.BasicConsumeAsync(
                queue: queueName,
                autoAck: true,
                consumer: consumer
            );

            while ( !stoppingToken.IsCancellationRequested )
            {
                await Task.Delay( 1000, stoppingToken );
            }
        }

        public async Task<OrderRequestDto> PublishOrderAsync( OrderRequestDto order, string queueName )
        {
            await InitAsync( );

            await _Channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            string message = JsonConvert.SerializeObject( order );
            var body = Encoding.UTF8.GetBytes( message );

            var properties = new BasicProperties( );

            await _Channel.BasicPublishAsync(
                exchange: "",
                routingKey: queueName,
                mandatory: false,
                basicProperties: properties,
                body: body,
                cancellationToken: CancellationToken.None
            );

            return order;
        }

        public async Task CloseConnectionAsync( )
        {
            if ( _Channel != null )
            {
                await _Channel.CloseAsync( );
            }

            if ( _Connection != null )
            {
                await _Connection.CloseAsync( );
            }
        }
    }
}
