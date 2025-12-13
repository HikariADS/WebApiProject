# Danh sách cải tiến cho dự án

## 🔴 QUAN TRỌNG - Nên làm ngay

### 1. **Bảo mật (Security)**
- ✅ **CORS Policy quá rộng**: Hiện tại `AllowAll` cho phép mọi origin. Nên giới hạn cho frontend URL cụ thể.
- ✅ **JWT Key và Connection String**: Đang hardcode trong `appsettings.json`. Nên dùng Environment Variables hoặc User Secrets.
- ✅ **Email Password exposed**: Password SMTP đang lộ trong `appsettings.json`. Nên dùng environment variables.
- ✅ **Password Policy yếu**: Không yêu cầu chữ số, chữ hoa. Nên tăng cường cho production.
- ⚠️ **RequireHttpsMetadata = false**: Không an toàn cho production. Nên bật HTTPS.

### 2. **Error Handling & Logging**
- ⚠️ **Thiếu try-catch trong Controllers**: Nhiều controller không có try-catch, dễ crash khi có lỗi.
- ⚠️ **Console.WriteLine thay vì ILogger**: Nên dùng `ILogger<T>` thay vì `Console.WriteLine`.
- ⚠️ **Console.log trong Frontend**: Có 24 instances của `console.log/error/warn`. Nên xóa hoặc dùng logging service.

### 3. **Performance**
- ⚠️ **StorageController load tất cả users**: `await _context.Users.ToListAsync()` load toàn bộ users vào memory. Nên chỉ query những users cần thiết.
- ✅ **ProductRepository.GetByIdAsync**: Đã có Include ProductType (OK).

## 🟡 QUAN TRỌNG VỪA - Nên làm sớm

### 4. **Code Quality**
- **Validation**: Thêm validation attributes vào DTOs (Required, Range, EmailAddress, etc.)
- **API Response Standardization**: Tạo wrapper cho API responses (success/error format thống nhất)
- **Null Safety**: Thêm null checks ở các nơi quan trọng

### 5. **User Experience**
- **Loading States**: Một số trang chưa có loading indicator rõ ràng
- **Error Messages**: Một số error messages chưa thân thiện với người dùng
- **Form Validation**: Cải thiện validation messages trên frontend

## 🟢 TỐT ĐỂ CÓ - Có thể làm sau

### 6. **Best Practices**
- **API Versioning**: Thêm versioning cho API (v1, v2)
- **Rate Limiting**: Giới hạn số request để tránh abuse
- **Caching**: Thêm caching cho các query thường dùng
- **Unit Tests**: Thêm unit tests cho services và repositories
- **Integration Tests**: Thêm integration tests cho API endpoints

### 7. **Documentation**
- **API Documentation**: Cải thiện Swagger documentation với examples
- **Code Comments**: Thêm XML comments cho public APIs
- **README**: Cập nhật README với setup instructions chi tiết

---

## Ưu tiên thực hiện

1. **Bảo mật**: CORS, Environment Variables, Password Policy
2. **Error Handling**: Try-catch trong controllers, ILogger
3. **Performance**: Tối ưu query trong StorageController
4. **Code Quality**: Validation, API Response Standardization

