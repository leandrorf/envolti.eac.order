using envolti.lib.order.application.Mediator.Interfaces;
using envolti.lib.order.application.Order.Responses;
using envolti.lib.order.domain.Order.Dtos;
using envolti.lib.order.domain.Order.Enums;
using envolti.lib.order.domain.Order.Exceptions;
using envolti.lib.order.domain.Order.Ports;

namespace envolti.lib.order.application.Order.Queries
{
    public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, OrderListResponse>
    {
        private readonly IOrderRepository _OrderRepository;
        private readonly IOrderCacheAdapter _OrderRedisAdapter;

        public GetAllOrdersQueryHandler( IOrderRepository orderRepository, IOrderCacheAdapter orderRedisAdapter )
        {
            _OrderRepository = orderRepository;
            _OrderRedisAdapter = orderRedisAdapter;
        }

        public async Task<OrderListResponse> Handle( GetAllOrdersQuery request, CancellationToken cancellationToken )
        {
            try
            {
                var resp = await _OrderRedisAdapter.ConsumerOrderAllAsync<OrderResponseDto>( request.PageNumber, request.PageSize );

                if ( resp.Items.Any( ) )
                {
                    var orders = await _OrderRepository.GetAllAsync( request.PageNumber, request.PageSize );

                    if ( orders != null && orders.Items.Any( ) )
                    {
                        var items = orders.Items.Select( o => o.MapEntityToDto( ) ).ToList( );

                        resp = new PagedResult<OrderResponseDto>
                        {
                            Items = items,
                            Total = orders.Total,
                            PageNumber = request.PageNumber,
                            PageSize = request.PageSize
                        };

                        await _OrderRedisAdapter.PublishOrderAsync( items );
                    }
                }

                if ( !resp.Items.Any( ) )
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
