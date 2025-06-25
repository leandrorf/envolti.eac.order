using envolti.lib.order.application.Mediator.Interfaces;
using envolti.lib.order.application.Order.Responses;

namespace envolti.lib.order.application.Order.Queries
{
    public class GetOrderByIdQuery : IRequest<OrderSingleResponse>
    {
        public int OrderIdExternal { get; set; }
    }
}
