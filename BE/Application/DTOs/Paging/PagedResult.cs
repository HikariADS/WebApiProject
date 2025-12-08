namespace WebApiProject.Application.DTOs.Paging
{
    public class PageResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();  
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
       
    }
}
