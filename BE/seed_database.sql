-- Script để xóa dữ liệu cũ và seed dữ liệu mới vào database
USE WarehouseDb;
GO

-- Xóa dữ liệu cũ (theo thứ tự để tránh foreign key constraint)
DELETE FROM Storages;
DELETE FROM Products;
DELETE FROM ProductTypes;
DELETE FROM StorageTypes;
DELETE FROM AspNetUserRoles;
DELETE FROM AspNetUsers;
DELETE FROM AspNetRoles;
DELETE FROM PendingRegistrations;
GO

-- Seed Roles
INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
VALUES 
    (NEWID(), 'Admin', 'ADMIN', NEWID()),
    (NEWID(), 'Manager', 'MANAGER', NEWID()),
    (NEWID(), 'User', 'USER', NEWID());
GO

-- Seed ProductTypes
INSERT INTO ProductTypes (Name, Description)
VALUES 
    ('Electronics', 'Điện tử và thiết bị điện'),
    ('Toys', 'Đồ chơi'),
    ('Clothing', 'Quần áo'),
    ('Accessories', 'Phụ kiện'),
    ('Sports', 'Thể thao');
GO

-- Seed StorageTypes
INSERT INTO StorageTypes (Name, ManagerName, StorageLocation)
VALUES 
    ('Kho chính', 'Nguyễn Văn A', '123 Đường ABC, Quận 1, TP.HCM'),
    ('Kho phụ', 'Trần Thị B', '456 Đường XYZ, Quận 2, TP.HCM'),
    ('Kho trưng bày', 'Lê Văn C', '789 Đường DEF, Quận 3, TP.HCM'),
    ('Kho miền Bắc', 'Phạm Thị D', '321 Đường GHI, Hà Nội'),
    ('Kho miền Trung', 'Hoàng Văn E', '654 Đường JKL, Đà Nẵng');
GO

-- Seed Users (sử dụng Identity hash cho password)
-- Password: Admin@123, Manager@123, User@123
DECLARE @AdminId NVARCHAR(450) = NEWID();
DECLARE @ManagerId NVARCHAR(450) = NEWID();
DECLARE @UserId NVARCHAR(450) = NEWID();
DECLARE @AdminRoleId NVARCHAR(450) = (SELECT Id FROM AspNetRoles WHERE Name = 'Admin');
DECLARE @ManagerRoleId NVARCHAR(450) = (SELECT Id FROM AspNetRoles WHERE Name = 'Manager');
DECLARE @UserRoleId NVARCHAR(450) = (SELECT Id FROM AspNetRoles WHERE Name = 'User');

-- Insert Admin user
INSERT INTO AspNetUsers (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, FullName, UnitId)
VALUES 
    (@AdminId, 'admin', 'ADMIN', 'admin@local.com', 'ADMIN@LOCAL.COM', 1, 
     'AQAAAAIAAYagAAAAEHy8qJZ5qJZ5qJZ5qJZ5qJZ5qJZ5qJZ5qJZ5qJZ5qJZ5qJZ5qJZ5qJZ5qJZ5qJZ5qA==', 
     NEWID(), NEWID(), NULL, 0, 0, 1, 0, 'Quản trị viên', 'UNIT-001');

-- Insert Manager user
INSERT INTO AspNetUsers (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, FullName, UnitId)
VALUES 
    (@ManagerId, 'manager', 'MANAGER', 'manager@local.com', 'MANAGER@LOCAL.COM', 1,
     'AQAAAAIAAYagAAAAEHy8qJZ5qJZ5qJZ5qJZ5qJZ5qJZ5qJZ5qJZ5qJZ5qJZ5qJZ5qJZ5qJZ5qJZ5qJZ5qA==',
     NEWID(), NEWID(), NULL, 0, 0, 1, 0, 'Người quản lý', 'UNIT-001');

-- Insert User
INSERT INTO AspNetUsers (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, FullName, UnitId)
VALUES 
    (@UserId, 'user', 'USER', 'user@local.com', 'USER@LOCAL.COM', 1,
     'AQAAAAIAAYagAAAAEHy8qJZ5qJZ5qJZ5qJZ5qJZ5qJZ5qJZ5qJZ5qJZ5qJZ5qJZ5qJZ5qJZ5qJZ5qJZ5qA==',
     NEWID(), NEWID(), NULL, 0, 0, 1, 0, 'Người dùng', 'UNIT-001');

-- Assign roles
INSERT INTO AspNetUserRoles (UserId, RoleId)
VALUES 
    (@AdminId, @AdminRoleId),
    (@ManagerId, @ManagerRoleId),
    (@UserId, @UserRoleId);
GO

-- Seed Products (30 products)
DECLARE @ProductType1 INT = (SELECT TOP 1 Id FROM ProductTypes WHERE Name = 'Electronics');
DECLARE @ProductType2 INT = (SELECT TOP 1 Id FROM ProductTypes WHERE Name = 'Toys');
DECLARE @ProductType3 INT = (SELECT TOP 1 Id FROM ProductTypes WHERE Name = 'Clothing');
DECLARE @ProductType4 INT = (SELECT TOP 1 Id FROM ProductTypes WHERE Name = 'Accessories');
DECLARE @ProductType5 INT = (SELECT TOP 1 Id FROM ProductTypes WHERE Name = 'Sports');

INSERT INTO Products (Name, Description, Price, ProductTypeId, Quantity)
VALUES 
    ('Laptop Dell XPS 15', 'Laptop cao cấp với màn hình 15 inch', 25000000, @ProductType1, 50),
    ('iPhone 15 Pro', 'Điện thoại thông minh mới nhất', 30000000, @ProductType1, 30),
    ('Samsung TV 55 inch', 'Smart TV 4K UHD', 15000000, @ProductType1, 20),
    ('AirPods Pro', 'Tai nghe không dây chống ồn', 5000000, @ProductType1, 100),
    ('iPad Air', 'Máy tính bảng đa năng', 18000000, @ProductType1, 40),
    ('Xe đạp trẻ em', 'Xe đạp 2 bánh cho trẻ 5-8 tuổi', 1500000, @ProductType2, 25),
    ('Lego Classic', 'Bộ xếp hình Lego 1000 mảnh', 2000000, @ProductType2, 60),
    ('Búp bê Barbie', 'Búp bê thời trang', 500000, @ProductType2, 80),
    ('Xe điều khiển từ xa', 'Xe RC điều khiển từ xa', 3000000, @ProductType2, 35),
    ('Bộ đồ chơi xếp hình', 'Đồ chơi giáo dục', 800000, @ProductType2, 50),
    ('Áo sơ mi nam', 'Áo sơ mi công sở', 500000, @ProductType3, 100),
    ('Quần jean nữ', 'Quần jean cao cấp', 800000, @ProductType3, 80),
    ('Áo khoác gió', 'Áo khoác chống nước', 1200000, @ProductType3, 60),
    ('Váy liền thân', 'Váy công sở', 900000, @ProductType3, 70),
    ('Áo thun nam', 'Áo thun cotton', 300000, @ProductType3, 150),
    ('Túi xách da', 'Túi xách da thật', 2500000, @ProductType4, 40),
    ('Ví da nam', 'Ví da cao cấp', 800000, @ProductType4, 100),
    ('Đồng hồ thông minh', 'Smartwatch', 5000000, @ProductType4, 50),
    ('Kính mát', 'Kính mát chống tia UV', 1500000, @ProductType4, 60),
    ('Thắt lưng da', 'Thắt lưng da thật', 600000, @ProductType4, 80),
    ('Giày thể thao', 'Giày chạy bộ', 2000000, @ProductType5, 70),
    ('Vợt cầu lông', 'Vợt cầu lông chuyên nghiệp', 1500000, @ProductType5, 50),
    ('Bóng đá', 'Bóng đá size 5', 500000, @ProductType5, 100),
    ('Quần áo thể thao', 'Bộ đồ tập gym', 800000, @ProductType5, 90),
    ('Mũ bảo hiểm', 'Mũ bảo hiểm xe đạp', 600000, @ProductType5, 60),
    ('Máy chạy bộ', 'Máy tập thể dục tại nhà', 12000000, @ProductType5, 15),
    ('Tạ tay', 'Bộ tạ tay 20kg', 1500000, @ProductType5, 40),
    ('Yoga mat', 'Thảm tập yoga', 400000, @ProductType5, 80),
    ('Bình nước thể thao', 'Bình nước 1L', 200000, @ProductType5, 120),
    ('Balo thể thao', 'Balo du lịch', 1000000, @ProductType5, 55);
GO

-- Seed Storages (20 records)
DECLARE @AdminUserId NVARCHAR(450) = (SELECT Id FROM AspNetUsers WHERE UserName = 'admin');
DECLARE @ManagerUserId NVARCHAR(450) = (SELECT Id FROM AspNetUsers WHERE UserName = 'manager');
DECLARE @StorageType1 INT = (SELECT TOP 1 Id FROM StorageTypes WHERE Name = 'Kho chính');
DECLARE @StorageType2 INT = (SELECT TOP 1 Id FROM StorageTypes WHERE Name = 'Kho phụ');
DECLARE @StorageType3 INT = (SELECT TOP 1 Id FROM StorageTypes WHERE Name = 'Kho trưng bày');
DECLARE @StorageType4 INT = (SELECT TOP 1 Id FROM StorageTypes WHERE Name = 'Kho miền Bắc');
DECLARE @StorageType5 INT = (SELECT TOP 1 Id FROM StorageTypes WHERE Name = 'Kho miền Trung');

-- Lấy một số Product IDs
DECLARE @ProductIds TABLE (Id INT);
INSERT INTO @ProductIds SELECT TOP 20 Id FROM Products ORDER BY Id;

DECLARE @ProductId1 INT = (SELECT TOP 1 Id FROM @ProductIds ORDER BY Id);
DECLARE @ProductId2 INT = (SELECT TOP 1 Id FROM @ProductIds ORDER BY Id OFFSET 1 ROWS FETCH NEXT 1 ROWS ONLY);
DECLARE @ProductId3 INT = (SELECT TOP 1 Id FROM @ProductIds ORDER BY Id OFFSET 2 ROWS FETCH NEXT 1 ROWS ONLY);
DECLARE @ProductId4 INT = (SELECT TOP 1 Id FROM @ProductIds ORDER BY Id OFFSET 3 ROWS FETCH NEXT 1 ROWS ONLY);
DECLARE @ProductId5 INT = (SELECT TOP 1 Id FROM @ProductIds ORDER BY Id OFFSET 4 ROWS FETCH NEXT 1 ROWS ONLY);
DECLARE @ProductId6 INT = (SELECT TOP 1 Id FROM @ProductIds ORDER BY Id OFFSET 5 ROWS FETCH NEXT 1 ROWS ONLY);
DECLARE @ProductId7 INT = (SELECT TOP 1 Id FROM @ProductIds ORDER BY Id OFFSET 6 ROWS FETCH NEXT 1 ROWS ONLY);
DECLARE @ProductId8 INT = (SELECT TOP 1 Id FROM @ProductIds ORDER BY Id OFFSET 7 ROWS FETCH NEXT 1 ROWS ONLY);
DECLARE @ProductId9 INT = (SELECT TOP 1 Id FROM @ProductIds ORDER BY Id OFFSET 8 ROWS FETCH NEXT 1 ROWS ONLY);
DECLARE @ProductId10 INT = (SELECT TOP 1 Id FROM @ProductIds ORDER BY Id OFFSET 9 ROWS FETCH NEXT 1 ROWS ONLY);
DECLARE @ProductId11 INT = (SELECT TOP 1 Id FROM @ProductIds ORDER BY Id OFFSET 10 ROWS FETCH NEXT 1 ROWS ONLY);
DECLARE @ProductId12 INT = (SELECT TOP 1 Id FROM @ProductIds ORDER BY Id OFFSET 11 ROWS FETCH NEXT 1 ROWS ONLY);
DECLARE @ProductId13 INT = (SELECT TOP 1 Id FROM @ProductIds ORDER BY Id OFFSET 12 ROWS FETCH NEXT 1 ROWS ONLY);
DECLARE @ProductId14 INT = (SELECT TOP 1 Id FROM @ProductIds ORDER BY Id OFFSET 13 ROWS FETCH NEXT 1 ROWS ONLY);
DECLARE @ProductId15 INT = (SELECT TOP 1 Id FROM @ProductIds ORDER BY Id OFFSET 14 ROWS FETCH NEXT 1 ROWS ONLY);
DECLARE @ProductId16 INT = (SELECT TOP 1 Id FROM @ProductIds ORDER BY Id OFFSET 15 ROWS FETCH NEXT 1 ROWS ONLY);
DECLARE @ProductId17 INT = (SELECT TOP 1 Id FROM @ProductIds ORDER BY Id OFFSET 16 ROWS FETCH NEXT 1 ROWS ONLY);
DECLARE @ProductId18 INT = (SELECT TOP 1 Id FROM @ProductIds ORDER BY Id OFFSET 17 ROWS FETCH NEXT 1 ROWS ONLY);
DECLARE @ProductId19 INT = (SELECT TOP 1 Id FROM @ProductIds ORDER BY Id OFFSET 18 ROWS FETCH NEXT 1 ROWS ONLY);
DECLARE @ProductId20 INT = (SELECT TOP 1 Id FROM @ProductIds ORDER BY Id OFFSET 19 ROWS FETCH NEXT 1 ROWS ONLY);

INSERT INTO Storages (ProductId, Quantity, UserId, StorageTypeId, ImportDate, ExportDate, BelongToUnitId, ManagerId)
VALUES 
    (@ProductId1, 50, @AdminUserId, @StorageType1, DATEADD(day, -30, GETDATE()), NULL, 'UNIT-001', @ManagerUserId),
    (@ProductId2, 30, @AdminUserId, @StorageType1, DATEADD(day, -25, GETDATE()), DATEADD(day, -5, GETDATE()), 'UNIT-001', @ManagerUserId),
    (@ProductId3, 20, @AdminUserId, @StorageType2, DATEADD(day, -20, GETDATE()), NULL, 'UNIT-001', @ManagerUserId),
    (@ProductId4, 100, @AdminUserId, @StorageType2, DATEADD(day, -15, GETDATE()), DATEADD(day, -2, GETDATE()), 'UNIT-001', @ManagerUserId),
    (@ProductId5, 40, @AdminUserId, @StorageType3, DATEADD(day, -10, GETDATE()), NULL, 'UNIT-001', @ManagerUserId),
    (@ProductId6, 25, @AdminUserId, @StorageType3, DATEADD(day, -8, GETDATE()), NULL, 'UNIT-001', @ManagerUserId),
    (@ProductId7, 60, @AdminUserId, @StorageType4, DATEADD(day, -45, GETDATE()), DATEADD(day, -10, GETDATE()), 'UNIT-001', @ManagerUserId),
    (@ProductId8, 80, @AdminUserId, @StorageType4, DATEADD(day, -40, GETDATE()), NULL, 'UNIT-001', @ManagerUserId),
    (@ProductId9, 35, @AdminUserId, @StorageType5, DATEADD(day, -35, GETDATE()), DATEADD(day, -7, GETDATE()), 'UNIT-001', @ManagerUserId),
    (@ProductId10, 70, @AdminUserId, @StorageType5, DATEADD(day, -28, GETDATE()), NULL, 'UNIT-001', @ManagerUserId),
    (@ProductId11, 45, @AdminUserId, @StorageType1, DATEADD(day, -22, GETDATE()), NULL, 'UNIT-001', @ManagerUserId),
    (@ProductId12, 55, @AdminUserId, @StorageType1, DATEADD(day, -18, GETDATE()), DATEADD(day, -3, GETDATE()), 'UNIT-001', @ManagerUserId),
    (@ProductId13, 65, @AdminUserId, @StorageType2, DATEADD(day, -12, GETDATE()), NULL, 'UNIT-001', @ManagerUserId),
    (@ProductId14, 75, @AdminUserId, @StorageType2, DATEADD(day, -7, GETDATE()), NULL, 'UNIT-001', @ManagerUserId),
    (@ProductId15, 85, @AdminUserId, @StorageType3, DATEADD(day, -5, GETDATE()), NULL, 'UNIT-001', @ManagerUserId),
    (@ProductId16, 90, @AdminUserId, @StorageType3, DATEADD(day, -3, GETDATE()), DATEADD(day, -1, GETDATE()), 'UNIT-001', @ManagerUserId),
    (@ProductId17, 95, @AdminUserId, @StorageType4, DATEADD(day, -50, GETDATE()), NULL, 'UNIT-001', @ManagerUserId),
    (@ProductId18, 110, @AdminUserId, @StorageType4, DATEADD(day, -42, GETDATE()), DATEADD(day, -8, GETDATE()), 'UNIT-001', @ManagerUserId),
    (@ProductId19, 120, @AdminUserId, @StorageType5, DATEADD(day, -38, GETDATE()), NULL, 'UNIT-001', @ManagerUserId),
    (@ProductId20, 130, @AdminUserId, @StorageType5, DATEADD(day, -33, GETDATE()), DATEADD(day, -6, GETDATE()), 'UNIT-001', @ManagerUserId);
GO

PRINT 'Database seeding completed successfully!';
PRINT 'Users created:';
PRINT '  - Admin: admin@local.com / Admin@123';
PRINT '  - Manager: manager@local.com / Manager@123';
PRINT '  - User: user@local.com / User@123';
GO

