namespace Horizon.MVC.DTOs
{
    public class PagedResponseDto<T>
    {
        public List<T> Data { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }
}


