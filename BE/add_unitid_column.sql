-- Script to add UnitId column to AspNetUsers table
-- Run this script directly in SQL Server Management Studio or sqlcmd

USE WarehouseDb;
GO

-- Check if column exists, if not add it
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUsers]') AND name = 'UnitId')
BEGIN
    ALTER TABLE [dbo].[AspNetUsers] ADD [UnitId] NVARCHAR(MAX) NULL;
    PRINT 'Column UnitId added successfully to AspNetUsers table';
END
ELSE
BEGIN
    PRINT 'Column UnitId already exists in AspNetUsers table';
END
GO

