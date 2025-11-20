namespace WebApiProject.Application.DTOs.Paging
{
    public class PageRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Optional Search
        public string? Search { get; set; }

        // Optional Filter
        public int? ProductTypeId { get; set; }

        // Sorting
        public string? SortBy { get; set; }  // vd: "name", "price"
    
        public string SortOrder { get; set; } = "asc"; // asc/desc
    }
}
