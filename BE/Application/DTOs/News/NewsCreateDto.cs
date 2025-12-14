using System.ComponentModel.DataAnnotations;

namespace WebApiProject.Application.DTOs.News
{
    public class NewsCreateDto
    {
        [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Tiêu đề phải từ 5 đến 200 ký tự")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nội dung là bắt buộc")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Nội dung phải từ 10 đến 2000 ký tự")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Danh mục là bắt buộc")]
        [StringLength(50)]
        public string Category { get; set; } = "Thông báo";
    }
}

