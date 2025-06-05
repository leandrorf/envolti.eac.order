using envolti.lib.order.domain.Order.Dtos;

namespace envolti.lib.order.application.Order.Responses
{
    public class OrderResponse : Response
    {
        public required OrderResponseDto Data;
    }
}
