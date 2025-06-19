using envolti.lib.order.application.Order.Responses;
using envolti.lib.order.domain.Order.Dtos;
using MediatR;

namespace envolti.lib.order.application.Order.Commands
{
    public class PublishOrderCommand : IRequest<OrderQueueResponse>
    {
        public required OrderRequestDto Data { get; set; }
    }
}
