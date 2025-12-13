# Hướng dẫn cập nhật ManagerId cho các Storage hiện có

## Cách 1: Sử dụng API Endpoint (Khuyến nghị)

1. Đăng nhập với tài khoản Admin
2. Gọi endpoint: `POST /api/storage/update-manager-ids`
3. Endpoint này sẽ tự động cập nhật tất cả Storage có ManagerId = null với Manager user đầu tiên tìm được

## Cách 2: Sử dụng SQL Script

Chạy script SQL sau trong SQL Server Management Studio hoặc Azure Data Studio:

```sql
-- Script to update ManagerId for existing Storages
DECLARE @ManagerId NVARCHAR(450);
SELECT @ManagerId = Id FROM AspNetUsers WHERE UserName = 'manager' OR Email = 'manager@local.com';

IF @ManagerId IS NOT NULL
BEGIN
    UPDATE Storages
    SET ManagerId = @ManagerId
    WHERE ManagerId IS NULL OR ManagerId = '';
    
    SELECT 'Updated ' + CAST(@@ROWCOUNT AS VARCHAR) + ' storage records with ManagerId' AS Result;
END
ELSE
BEGIN
    SELECT 'Manager user not found. Please create manager user first.' AS Result;
END
```

## Lưu ý

- Endpoint chỉ có thể gọi bởi Admin
- Script sẽ cập nhật tất cả Storage có ManagerId null hoặc rỗng
- Nếu có nhiều Manager, script sẽ chọn Manager đầu tiên tìm được

