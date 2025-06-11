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

        public PublishOrderCommandHandler( IOrderQueuesAdapter orderAdapter )
        {
            _OrderAdapter = orderAdapter;
        }

        public async Task<OrderResponse> Handle( PublishOrderCommand request, CancellationToken cancellationToken )
        {
            try
            {
                var orderDto = request.Data;
                var order = OrderQueuesAdapter.MapToAdapter( orderDto );

                await order.Save( _OrderAdapter );

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
