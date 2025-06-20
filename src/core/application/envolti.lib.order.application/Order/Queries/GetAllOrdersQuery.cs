using envolti.lib.order.domain.Order.Dtos;
using MediatR;

namespace envolti.lib.order.application.Order.Queries
{
    public class GetAllOrdersQuery : IRequest<IEnumerable<OrderResponseDto>>
    {
        public required int PageNumber { get; set; }
        public required int PageSize { get; set; }
    }
}
