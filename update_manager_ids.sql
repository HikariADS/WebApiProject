-- Script to update ManagerId for existing Storages
-- First, get the Manager user ID
DECLARE @ManagerId NVARCHAR(450);
SELECT @ManagerId = Id FROM AspNetUsers WHERE UserName = 'manager' OR Email = 'manager@local.com';

-- If manager exists, update all Storages that have NULL ManagerId
IF @ManagerId IS NOT NULL
BEGIN
    UPDATE Storages
    SET ManagerId = @ManagerId
    WHERE ManagerId IS NULL;
    
    PRINT 'Updated ' + CAST(@@ROWCOUNT AS VARCHAR) + ' storage records with ManagerId';
END
ELSE
BEGIN
    PRINT 'Manager user not found. Please create manager user first.';
END
