using envolti.lib.order.domain.Order.Dtos;

namespace envolti.lib.order.application.Order.Responses
{
    public class OrderListResponse : Response
    {
        public PagedResult<OrderResponseDto>? Data { get; set; }
    }
}
