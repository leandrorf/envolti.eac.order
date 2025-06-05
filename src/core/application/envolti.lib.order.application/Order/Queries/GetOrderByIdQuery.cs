using envolti.lib.order.application.Order.Responses;
using MediatR;

namespace envolti.lib.order.application.Order.Queries
{
    public class GetOrderByIdQuery : IRequest<OrderResponse>
    {
        public int OrderIdExternal { get; set; }
    }
}
