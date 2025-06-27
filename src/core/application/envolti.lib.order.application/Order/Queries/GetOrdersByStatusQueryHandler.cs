using envolti.lib.order.application.Mediator.Interfaces;
using envolti.lib.order.application.Order.Responses;
using envolti.lib.order.domain.Order.Dtos;
using envolti.lib.order.domain.Order.Enums;
using envolti.lib.order.domain.Order.Exceptions;
using envolti.lib.order.domain.Order.Ports;

namespace envolti.lib.order.application.Order.Queries
{
    public class GetOrdersByStatusQueryHandler : IRequestHandler<GetOrdersByStatusQuery, OrderListResponse>
    {
        private readonly IOrderRepository _OrderRepository;
        private readonly IOrderCacheAdapter _OrderRedisAdapter;
        private readonly IOrderQueuesAdapter _OrderQueuesAdapter;

        public GetOrdersByStatusQueryHandler( IOrderRepository orderRepository, IOrderCacheAdapter orderRedisAdapter )
        {
            _OrderRepository = orderRepository;
            _OrderRedisAdapter = orderRedisAdapter;
        }

        public async Task<OrderListResponse> Handle( GetOrdersByStatusQuery request, CancellationToken cancellationToken )
        {
            try
            {
                var resp = await _OrderRedisAdapter.GetOrdersByStatusAsync<OrderResponseDto>( "status", request.Status, request.PageNumber, request.PageSize );


                if ( resp == null || resp.Items?.Any( ) == null )
                {
                    var orders = await _OrderRepository.GetOrdersByStatusAsync( request.Status, request.PageNumber, request.PageSize );

                    if ( orders?.Items != null && orders.Items.Any( ) )
                    {
                        await _OrderRedisAdapter.PublishOrderAsync( resp?.Items );
                    }
                }

                if ( resp?.Items?.Any( ) == null )
                {
                    throw new NoRecordsFoundException( );
                }

                return new OrderListResponse
                {
                    Data = resp,
                    Success = true,
                    Message = "Orders retrieved successfully."
                };
            }
            catch ( NoRecordsFoundException )
            {
                return new OrderListResponse
                {
                    Success = false,
                    ErrorCode = ErrorCodesResponseEnum.RECORD_NOT_FOUND,
                    Message = "No orders found."
                };
            }
            catch ( Exception ex )
            {
                return new OrderListResponse
                {
                    Success = false,
                    ErrorCode = ErrorCodesResponseEnum.UNIDENTIFIED_ERROR,
                    Message = $"An unexpected error occurred: {ex.Message}"
                };
            }
        }
    }
}