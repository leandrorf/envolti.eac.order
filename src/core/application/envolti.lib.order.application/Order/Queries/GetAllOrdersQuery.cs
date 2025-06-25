using envolti.lib.order.application.Mediator.Interfaces;
using envolti.lib.order.application.Order.Responses;

namespace envolti.lib.order.application.Order.Queries
{
    public class GetAllOrdersQuery : IRequest<OrderListResponse>
    {
        public required int PageNumber { get; set; }
        public required int PageSize { get; set; }
    }
}
