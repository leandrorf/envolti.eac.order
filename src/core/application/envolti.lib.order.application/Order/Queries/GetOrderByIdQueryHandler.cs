using envolti.lib.order.application.Order.Responses;
using envolti.lib.order.domain.Order.Dtos;
using envolti.lib.order.domain.Order.Ports;
using MediatR;

namespace envolti.lib.order.application.Order.Queries
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderResponse>
    {
        private readonly IOrderRepository _OrderRepository;
        private readonly IOrderRedisAdapter _OrderRedisAdapter;

        public GetOrderByIdQueryHandler( IOrderRepository orderRepository, IOrderRedisAdapter orderRedisAdapter )
        {
            _OrderRepository = orderRepository;
            _OrderRedisAdapter = orderRedisAdapter;
        }

        public async Task<OrderResponse> Handle( GetOrderByIdQuery request, CancellationToken cancellationToken )
        {
            OrderResponseDto resp = await _OrderRedisAdapter.ConsumerOrderByIdAsync<OrderResponseDto>(
                "orders", "JSON.GET", $"$.items[?(@.OrderIdExternal == {request.OrderIdExternal})]" );

            if ( resp == null )
            {
                var order = await _OrderRepository.GetOrderByIdAsync( request.OrderIdExternal );

                if ( order != null )
                {
                    resp = order.MapEntityToDto( );
                    await _OrderRedisAdapter.PublishOrderAsync( "orders", resp );

                }
            }

            // validar se order é nulo

            return new OrderResponse
            {
                Data = resp,
                Success = true,
                Message = "Order retrieved successfully."
            };
        }
    }
}
