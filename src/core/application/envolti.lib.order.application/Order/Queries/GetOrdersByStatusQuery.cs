using envolti.lib.order.domain.Order.Dtos;
using envolti.lib.order.domain.Order.Enums;
using MediatR;

namespace envolti.lib.order.application.Order.Queries
{
    public class GetOrdersByStatusQuery : IRequest<PagedResult<OrderResponseDto>>
    {
        public required int PageNumber { get; set; }
        public required int PageSize { get; set; }
        public StatusEnum Status { get; set; }
    }
}
