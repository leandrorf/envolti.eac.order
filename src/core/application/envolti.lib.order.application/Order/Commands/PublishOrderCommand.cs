using envolti.lib.order.application.Mediator.Interfaces;
using envolti.lib.order.application.Order.Responses;
using envolti.lib.order.domain.Order.Dtos;

namespace envolti.lib.order.application.Order.Commands
{
    public class PublishOrderCommand : IRequest<OrderQueueResponse>
    {
        public required OrderRequestDto Data { get; set; }
    }
}
