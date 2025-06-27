using envolti.lib.order.application.Mediator.Interfaces;
using envolti.lib.order.application.Order.Responses;
using envolti.lib.order.domain.Order.Dtos;
using envolti.lib.order.domain.Order.Enums;

namespace envolti.lib.order.application.Order.Queries
{
    public class GetOrdersByStatusQuery : IRequest<OrderListResponse>
    {
        public required int PageNumber { get; set; }
        public required int PageSize { get; set; }
        public StatusEnum Status { get; set; }
    }
}
