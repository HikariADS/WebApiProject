using WebApiProject.Application.DTOs.Paging;
using System.ComponentModel.DataAnnotations;

namespace WebApiProject.Application.DTOs.Product
{
    /// <summary>
    /// Tham số truy vấn cho Product, bao gồm Paging, Search, Filter và Sort.
    /// Kế thừa từ PagedRequest.
    /// </summary>
    public class ProductQueryRequest : PageRequest
    {
        // Kế thừa các thuộc tính: PageNumber, PageSize, Search, SortBy, SortOrder từ PagedRequest.

        // --- FILTER RIÊNG CHO PRODUCT ---
        
        // Lọc theo ID loại sản phẩm (ví dụ: ProductTypeId=2)
        public string? Keyword { get; set; }

        public string Id { get; set; } = string.Empty;
        
        // Lọc theo khoảng giá tối thiểu
        public decimal? MinPrice { get; set; }
        
        // Lọc theo khoảng giá tối đa
        public decimal? MaxPrice { get; set; }

        // Mở rộng tìm kiếm (nếu cần)
        // Nếu trường 'Search' (trong PagedRequest) không đủ, bạn có thể thêm các trường tìm kiếm chuyên biệt khác ở đây.
    }
}