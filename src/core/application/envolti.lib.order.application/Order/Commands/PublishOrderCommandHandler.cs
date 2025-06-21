using envolti.lib.order.application.Order.Responses;
using envolti.lib.order.domain.Order.Adapters;
using envolti.lib.order.domain.Order.Dtos;
using envolti.lib.order.domain.Order.Enums;
using envolti.lib.order.domain.Order.Exceptions;
using envolti.lib.order.domain.Order.Ports;
using MediatR;

namespace envolti.lib.order.application.Order.Commands
{
    public class PublishOrderCommandHandler : IRequestHandler<PublishOrderCommand, OrderQueueResponse>
    {
        private readonly IOrderQueuesAdapter _OrderAdapter;

        public PublishOrderCommandHandler( IOrderQueuesAdapter orderAdapter )
        {
            _OrderAdapter = orderAdapter;
        }

        public async Task<OrderQueueResponse> Handle( PublishOrderCommand request, CancellationToken cancellationToken )
        {
            try
            {
                var orderDto = request.Data;
                var order = OrderQueuesAdapter.MapToAdapter( request.Data );

                await order.Save( _OrderAdapter );

                return new OrderQueueResponse
                {
                    Data = order.MapAdapterToDto( ),
                    Success = true,
                    Message = "Registration request successful."
                };
            }
            catch ( TheOrderNumberCannotBeRepeatedException )
            {
                return new OrderQueueResponse
                {
                    Data = null!,
                    Success = false,
                    Message = "The order number cannot be repeated.",
                    ErrorCode = ErrorCodesResponseEnum.THE_ORDER_NUMBER_CANNOT_BE_REPEATED
                };
            }
            catch ( Exception ex )
            {
                return new OrderQueueResponse
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
