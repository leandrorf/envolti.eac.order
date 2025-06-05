using envolti.lib.order.domain.Order.Enums;

namespace envolti.lib.order.application
{
    public abstract class Response
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public ErrorCodesResponseEnum ErrorCode { get; set; }
    }
}
