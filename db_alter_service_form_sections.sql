-- ============================================================
--  db_alter_service_form_sections.sql
--  Migrates ServiceFormSections: FeatureCategoryId  →  ServiceId
--  Run once against BoulevardDb
-- ============================================================
USE BoulevardDb;
GO

-- ─── Step 1: Drop FK constraint on FeatureCategoryId (find name dynamically) ────
DECLARE @fkName NVARCHAR(256);

SELECT TOP 1 @fkName = f.name
FROM sys.foreign_keys f
INNER JOIN sys.foreign_key_columns fc  ON f.object_id = fc.constraint_object_id
INNER JOIN sys.columns c               ON fc.parent_column_id = c.column_id
                                       AND fc.parent_object_id = c.object_id
WHERE f.parent_object_id = OBJECT_ID('dbo.ServiceFormSections')
  AND c.name = 'FeatureCategoryId';

IF @fkName IS NOT NULL
    EXEC('ALTER TABLE dbo.ServiceFormSections DROP CONSTRAINT [' + @fkName + ']');
GO

-- ─── Step 2: Clear old seed data (linked to wrong concept) ──────────────────────
DELETE FROM dbo.ServiceFormAttachmentRules;
DELETE FROM dbo.ServiceFormFieldOptions;
DELETE FROM dbo.ServiceFormFields;
DELETE FROM dbo.ServiceFormSections;
GO

-- ─── Step 3: Drop the old FeatureCategoryId column ──────────────────────────────
IF EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.ServiceFormSections')
      AND name = 'FeatureCategoryId'
)
    ALTER TABLE dbo.ServiceFormSections DROP COLUMN FeatureCategoryId;
GO

-- ─── Step 4: Add ServiceId column ───────────────────────────────────────────────
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.ServiceFormSections')
      AND name = 'ServiceId'
)
    ALTER TABLE dbo.ServiceFormSections ADD ServiceId INT NOT NULL DEFAULT 0;
GO

-- ─── Step 5: Add FK → dbo.Services ──────────────────────────────────────────────
IF NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys
    WHERE name = 'FK_ServiceFormSections_Services'
      AND parent_object_id = OBJECT_ID('dbo.ServiceFormSections')
)
    ALTER TABLE dbo.ServiceFormSections
    ADD CONSTRAINT FK_ServiceFormSections_Services
        FOREIGN KEY (ServiceId) REFERENCES dbo.Services (ServiceId);
GO

PRINT 'Migration complete: ServiceFormSections now uses ServiceId FK → Services.';
