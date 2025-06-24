using envolti.lib.order.application.Order.Responses;
using MediatR;

namespace envolti.lib.order.application.Order.Queries
{
    public class GetOrderByIdQuery : IRequest<OrderSingleResponse>
    {
        public int OrderIdExternal { get; set; }
    }
}
