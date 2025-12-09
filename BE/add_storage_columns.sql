-- Add BelongToUnitId and ManagerId columns to Storages table
USE WarehouseDb;
GO

-- Check if columns exist before adding
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Storages]') AND name = 'BelongToUnitId')
BEGIN
    ALTER TABLE [dbo].[Storages]
    ADD [BelongToUnitId] NVARCHAR(MAX) NULL;
    PRINT 'Added BelongToUnitId column';
END
ELSE
BEGIN
    PRINT 'BelongToUnitId column already exists';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Storages]') AND name = 'ManagerId')
BEGIN
    ALTER TABLE [dbo].[Storages]
    ADD [ManagerId] NVARCHAR(MAX) NULL;
    PRINT 'Added ManagerId column';
END
ELSE
BEGIN
    PRINT 'ManagerId column already exists';
END
GO

