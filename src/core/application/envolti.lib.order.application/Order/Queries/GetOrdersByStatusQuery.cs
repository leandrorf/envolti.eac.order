using envolti.lib.order.domain.Order.Dtos;
using envolti.lib.order.domain.Order.Enums;
using MediatR;

namespace envolti.lib.order.application.Order.Queries
{
    public class GetOrdersByStatusQuery : IRequest<IEnumerable<OrderResponseDto>>
    {
        public StatusEnum Status { get; set; }
    }
}
