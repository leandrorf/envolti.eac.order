using envolti.lib.order.application.Mediator.Interfaces;
using envolti.lib.order.application.Order.Responses;
using envolti.lib.order.domain.Order.Dtos;
using envolti.lib.order.domain.Order.Enums;
using envolti.lib.order.domain.Order.Exceptions;
using envolti.lib.order.domain.Order.Ports;

namespace envolti.lib.order.application.Order.Queries
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderSingleResponse>
    {
        private readonly IOrderRepository _OrderRepository;
        private readonly IOrderCacheAdapter _OrderRedisAdapter;

        public GetOrderByIdQueryHandler( IOrderRepository orderRepository, IOrderCacheAdapter orderRedisAdapter )
        {
            _OrderRepository = orderRepository;
            _OrderRedisAdapter = orderRedisAdapter;
        }

        public async Task<OrderSingleResponse> Handle( GetOrderByIdQuery request, CancellationToken cancellationToken )
        {
            try
            {
                OrderResponseDto resp = await _OrderRedisAdapter.ConsumerOrderByIdAsync<OrderResponseDto>( "orderidexternal", request.OrderIdExternal );

                if ( resp == null )
                {
                    var order = await _OrderRepository.GetOrderByIdAsync( request.OrderIdExternal );

                    if ( order != null )
                    {
                        resp = order.MapEntityToDto( );
                        await _OrderRedisAdapter.PublishOrderAsync( resp );
                    }
                }

                return new OrderSingleResponse
                {
                    Data = resp,
                    Success = true,
                    Message = "Order retrieved successfully.",
                    ErrorCode = 0
                };
            }
            catch ( RecordNotFoundException )
            {
                return new OrderSingleResponse
                {
                    Data = null!,
                    Success = false,
                    Message = "Order not found.",
                    ErrorCode = ErrorCodesResponseEnum.RECORD_NOT_FOUND
                };
            }
            catch ( Exception ex )
            {
                return new OrderSingleResponse
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
