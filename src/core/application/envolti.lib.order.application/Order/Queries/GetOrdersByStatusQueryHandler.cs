using envolti.lib.order.domain.Order.Dtos;
using envolti.lib.order.domain.Order.Ports;
using MediatR;

namespace envolti.lib.order.application.Order.Queries
{
    public class GetOrdersByStatusQueryHandler : IRequestHandler<GetOrdersByStatusQuery, IEnumerable<OrderResponseDto>>
    {
        private readonly IOrderRepository _OrderRepository;
        private readonly IOrderRedisAdapter _OrderRedisAdapter;

        public GetOrdersByStatusQueryHandler( IOrderRepository orderRepository, IOrderRedisAdapter orderRedisAdapter )
        {
            _OrderRepository = orderRepository;
            _OrderRedisAdapter = orderRedisAdapter;
        }

        public async Task<IEnumerable<OrderResponseDto>> Handle( GetOrdersByStatusQuery request, CancellationToken cancellationToken )
        {
            List<OrderResponseDto> resp = await _OrderRedisAdapter.ConsumerOrderAllAsync<List<OrderResponseDto>>(
                "orders", "JSON.GET", $"$.items[?(@.Status == {( int )request.Status})]" );

            if ( resp == null || !resp.Any( ) )
            {
                var orders = await _OrderRepository.GetAllAsync( );

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