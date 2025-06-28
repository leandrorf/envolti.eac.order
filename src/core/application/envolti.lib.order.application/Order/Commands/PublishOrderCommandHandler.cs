using envolti.lib.order.application.Mediator.Interfaces;
using envolti.lib.order.application.Order.Responses;
using envolti.lib.order.domain.Order.Adapters;
using envolti.lib.order.domain.Order.Entities;
using envolti.lib.order.domain.Order.Enums;
using envolti.lib.order.domain.Order.Exceptions;
using envolti.lib.order.domain.Order.Ports;
using envolti.lib.order.domain.Order.Settings;
using Microsoft.Extensions.Options;

namespace envolti.lib.order.application.Order.Commands
{
    public class PublishOrderCommandHandler : IRequestHandler<PublishOrderCommand, OrderQueueResponse>
    {
        private readonly IOrderCacheAdapter _OrderCacheAdapter;
        private readonly IOrderQueuesAdapter _OrderAdapter;
        private readonly IOptions<RabbitMqSettings> _Settings;
        private readonly IOptions<RedisSettings> _RedisSettings;

        public PublishOrderCommandHandler( IOrderQueuesAdapter orderAdapter, IOptions<RabbitMqSettings> settings, IOrderCacheAdapter orderCacheAdapter )
        {
            _OrderAdapter = orderAdapter ?? throw new ArgumentNullException( nameof( orderAdapter ) );
            _Settings = settings ?? throw new ArgumentNullException( nameof( settings ) );
            _OrderCacheAdapter = orderCacheAdapter ?? throw new ArgumentNullException( nameof( orderCacheAdapter ) );
        }

        public async Task<OrderQueueResponse> Handle( PublishOrderCommand request, CancellationToken cancellationToken )
        {
            try
            {
                var orderDto = request.Data;
                var order = OrderQueuesAdapter.MapToAdapter( request.Data );
                var orderEntity = OrderQueuesAdapter.MapToEntity( orderDto );

                if ( _Settings.Value.Queue?.OrderQueue == null )
                {
                    throw new InvalidOperationException( "QueueOrder setting cannot be null." );
                }

                await order.Save( _OrderAdapter, _Settings.Value.Queue.OrderQueue );
                await _OrderCacheAdapter.PublishOrderAsync( orderEntity );

                return new OrderQueueResponse
                {
                    Data = order.MapAdapterToDto( ),
                    Success = true,
                    Message = "Registration request successful."
                };
            }
            catch ( TheOrderNumberCannotBeRepeatedException )
            {
                return new OrderQueueResponse
                {
                    Data = null!,
                    Success = false,
                    Message = "The order number cannot be repeated.",
                    ErrorCode = ErrorCodesResponseEnum.THE_ORDER_NUMBER_CANNOT_BE_REPEATED
                };
            }
            catch ( Exception ex )
            {
                return new OrderQueueResponse
                {
                    Data = null!,
                    Success = false,
                    Message = $"An unexpected error occurred: {ex.Message}",
                    ErrorCode = ErrorCodesResponseEnum.UNIDENTIFIED_ERROR
                };
            }
        }
    }
}
