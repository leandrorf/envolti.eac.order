using envolti.lib.order.domain.Order.Dtos;

namespace envolti.lib.order.application.Order.Responses
{
    public class OrderQueueResponse : Response
    {
        public required OrderResponseQueueDto Data;
    }
}
