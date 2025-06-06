using envolti.lib.order.application.Order.Responses;
using envolti.lib.order.domain.Order.Adapters;
using envolti.lib.order.domain.Order.Enums;
using envolti.lib.order.domain.Order.Exceptions;
using envolti.lib.order.domain.Order.Ports;
using MediatR;

namespace envolti.lib.order.application.Order.Commands
{
    public class PublishOrderCommandHandler : IRequestHandler<PublishOrderCommand, OrderResponse>
    {
        private readonly IOrderQueuesAdapter _OrderAdapter;
        private readonly IOrderRepository _OrderRepository;

        public PublishOrderCommandHandler( IOrderQueuesAdapter orderAdapter, IOrderRepository orderRepository )
        {
            _OrderAdapter = orderAdapter;
            _OrderRepository = orderRepository;
        }

        public async Task<OrderResponse> Handle( PublishOrderCommand request, CancellationToken cancellationToken )
        {
            try
            {
                var orderDto = request.Data;
                var order = OrderQueuesAdapter.MapToAdapter( orderDto );

                await order.Save( _OrderAdapter, _OrderRepository );

                return new OrderResponse
                {
                    Data = null,
                    Success = true,
                    Message = "Registration request successful."
                };
            }
            catch ( TheOrderNumberCannotBeRepeatedException )
            {
                return new OrderResponse
                {
                    Data = null,
                    Success = false,
                    Message = "The order number cannot be repeated.",
                    ErrorCode = ErrorCodesResponseEnum.THE_ORDER_NUMBER_CANNOT_BE_REPEATED
                };
            }
        }
    }
}
