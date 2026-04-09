USE BoulevardDb;
GO

-- =====================================================================
-- Migration v2: ServiceFormSections.ServiceId  -->  ServiceTypeId
-- Drop old FK to Services, drop ServiceId column,
-- add ServiceTypeId column with FK to ServiceTypes.
-- =====================================================================

-- 1. Drop existing FK on ServiceId
DECLARE @fkName NVARCHAR(256);
SELECT TOP 1 @fkName = f.name
FROM sys.foreign_keys f
JOIN sys.foreign_key_columns fc ON f.object_id = fc.constraint_object_id
JOIN sys.columns c ON fc.parent_column_id = c.column_id AND fc.parent_object_id = c.object_id
WHERE f.parent_object_id = OBJECT_ID('dbo.ServiceFormSections')
  AND c.name = 'ServiceId';

IF @fkName IS NOT NULL
    EXEC ('ALTER TABLE dbo.ServiceFormSections DROP CONSTRAINT [' + @fkName + ']');
GO

-- 2. Drop ServiceId column
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.ServiceFormSections') AND name = 'ServiceId')
    ALTER TABLE dbo.ServiceFormSections DROP COLUMN ServiceId;
GO

-- 3. Add ServiceTypeId column (default 0 so NOT NULL is satisfied for existing rows)
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.ServiceFormSections') AND name = 'ServiceTypeId')
    ALTER TABLE dbo.ServiceFormSections ADD ServiceTypeId INT NOT NULL DEFAULT 0;
GO

-- 4. Add FK to ServiceTypes
IF NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys
    WHERE name = 'FK_ServiceFormSections_ServiceTypes'
      AND parent_object_id = OBJECT_ID('dbo.ServiceFormSections')
)
    ALTER TABLE dbo.ServiceFormSections
        ADD CONSTRAINT FK_ServiceFormSections_ServiceTypes
        FOREIGN KEY (ServiceTypeId) REFERENCES dbo.ServiceTypes(ServiceTypeId);
GO

PRINT 'Migration v2 completed: ServiceFormSections now uses ServiceTypeId -> ServiceTypes';
GO
