-- Script để xóa dữ liệu cũ và chuẩn bị cho DataSeeder
SET QUOTED_IDENTIFIER ON;
GO

USE WarehouseDb;
GO

PRINT 'Starting to clear old data...';

-- Xóa dữ liệu cũ (theo thứ tự để tránh foreign key constraint)
DELETE FROM Storages;
PRINT 'Deleted all Storages';

DELETE FROM Products;
PRINT 'Deleted all Products';

DELETE FROM ProductTypes;
PRINT 'Deleted all ProductTypes';

DELETE FROM StorageTypes;
PRINT 'Deleted all StorageTypes';

DELETE FROM AspNetUserRoles;
PRINT 'Deleted all UserRoles';

DELETE FROM AspNetUsers;
PRINT 'Deleted all Users';

DELETE FROM AspNetRoles;
PRINT 'Deleted all Roles';

DELETE FROM PendingRegistrations;
PRINT 'Deleted all PendingRegistrations';

PRINT '';
PRINT 'All data cleared successfully!';
PRINT 'Please restart the backend application to run DataSeeder and populate new data.';
PRINT '';
PRINT 'Test accounts will be created:';
PRINT '  - Admin: admin@local.com / Admin@123';
PRINT '  - Manager: manager@local.com / Manager@123';
PRINT '  - User: user@local.com / User@123';
GO

