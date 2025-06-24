using envolti.lib.order.application.Order.Responses;
using envolti.lib.order.domain.Order.Dtos;
using MediatR;

namespace envolti.lib.order.application.Order.Queries
{
    //public class GetAllOrdersQuery : IRequest<PagedResult<OrderResponseDto>>
    public class GetAllOrdersQuery : IRequest<OrderListResponse>
    {
        public required int PageNumber { get; set; }
        public required int PageSize { get; set; }
    }
}
