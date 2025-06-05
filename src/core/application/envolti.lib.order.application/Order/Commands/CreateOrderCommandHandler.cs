using envolti.lib.order.application.Order.Responses;
using envolti.lib.order.domain.Order.Adapters;
using envolti.lib.order.domain.Order.Enums;
using envolti.lib.order.domain.Order.Exceptions;
using envolti.lib.order.domain.Order.Ports;
using MediatR;

namespace envolti.lib.order.application.Order.Commands
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderResponse>
    {
        private readonly IOrderRepository _OrderRepository;

        public CreateOrderCommandHandler( IOrderRepository orderRepository )
        {
            _OrderRepository = orderRepository;
        }

        public async Task<OrderResponse> Handle( CreateOrderCommand request, CancellationToken cancellationToken )
        {
            try
            {
                var orderDto = request.Data;
                var order = OrderQueuesAdapter.MapToEntity( orderDto );

                await order.Save( _OrderRepository );

                orderDto.Id = order.Id;

                return new OrderResponse
                {
                    Data = order.MapEntityToDto( ),
                    Success = true,
                    Message = "Order created successfully."
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
