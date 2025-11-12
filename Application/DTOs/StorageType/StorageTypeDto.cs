namespace WebApiProject.Application.DTOs.StorageType
{
    public class StorageTypeDto
    {
        public int Id { get; set; }

        // Tên loại kho, ví dụ "Kho lạnh", "Kho tổng"
        public string Name { get; set; } = string.Empty;

        // Người quản lý kho
        public string ManagerName { get; set; } = string.Empty;

        // Vị trí kho (ví dụ: “KCN Bắc Thăng Long”)
        public string StorageLocation { get; set; } = string.Empty;
    }
}
