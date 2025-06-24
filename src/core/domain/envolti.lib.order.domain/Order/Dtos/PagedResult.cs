namespace envolti.lib.order.domain.Order.Dtos
{
    public class PagedResult<T>
    {
        public int Total { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public IEnumerable<T>? Items { get; set; }
    }
}
