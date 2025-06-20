using envolti.lib.order.domain.Order.Dtos;
using envolti.lib.order.domain.Order.Ports;
using MediatR;

namespace envolti.lib.order.application.Order.Queries
{
    public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, IEnumerable<OrderResponseDto>>
    {
        private readonly IOrderRepository _OrderRepository;
        private readonly IOrderRedisAdapter _OrderRedisAdapter;

        public GetAllOrdersQueryHandler( IOrderRepository orderRepository, IOrderRedisAdapter orderRedisAdapter )
        {
            _OrderRepository = orderRepository;
            _OrderRedisAdapter = orderRedisAdapter;
        }

        public async Task<IEnumerable<OrderResponseDto>> Handle( GetAllOrdersQuery request, CancellationToken cancellationToken )
        {
            List<OrderResponseDto> resp = await _OrderRedisAdapter.ConsumerOrderAllAsync<List<OrderResponseDto>>( "orders", request.PageNumber, request.PageSize );

            if ( resp == null || !resp.Any( ) )
            {
                var orders = await _OrderRepository.GetAllAsync( request.PageNumber, request.PageSize );

                if ( orders != null && orders.Any( ) )
                {
                    resp = orders.Select( o => o.MapEntityToDto( ) ).ToList( );
                    await _OrderRedisAdapter.PublishOrderAsync( "orders", resp );
                }
            }

            return resp;
        }
    }
}
