using envolti.lib.order.application.Mediator.Interfaces;
using envolti.lib.order.domain.Order.Dtos;
using envolti.lib.order.domain.Order.Ports;

namespace envolti.lib.order.application.Order.Queries
{
    public class GetOrdersByStatusQueryHandler : IRequestHandler<GetOrdersByStatusQuery, PagedResult<OrderResponseDto>>
    {
        private readonly IOrderRepository _OrderRepository;
        private readonly IOrderCacheAdapter _OrderRedisAdapter;

        public GetOrdersByStatusQueryHandler( IOrderRepository orderRepository, IOrderCacheAdapter orderRedisAdapter )
        {
            _OrderRepository = orderRepository;
            _OrderRedisAdapter = orderRedisAdapter;
        }

        public async Task<PagedResult<OrderResponseDto>> Handle( GetOrdersByStatusQuery request, CancellationToken cancellationToken )
        {
            var resp = await _OrderRedisAdapter
                .GetOrdersByStatusAsync<OrderResponseDto>( "status", request.Status, request.PageNumber, request.PageSize );

            if ( resp == null || !resp.Items.Any( ) )
            {
                var orders = await _OrderRepository.GetOrdersByStatusAsync( request.Status, request.PageNumber, request.PageSize );

                if ( orders != null && orders.Items.Any( ) )
                {
                    await _OrderRedisAdapter.PublishOrderAsync( resp?.Items );
                }
            }

            return resp!;
        }
    }
}