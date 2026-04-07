-- ═══════════════════════════════════════════════════════════
--  Origin column — Social Impact Tracker
--  Adds "Origin" (e.g. Local, Imported) to Products and TempProducts
--  Run once against BoulevardDb
-- ═══════════════════════════════════════════════════════════

-- Products table
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'Products' AND COLUMN_NAME = 'Origin'
)
BEGIN
    ALTER TABLE dbo.Products ADD Origin NVARCHAR(100) NULL;
    PRINT 'Added Origin column to Products';
END
ELSE
    PRINT 'Origin column already exists in Products';

-- TempProducts table
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'TempProducts' AND COLUMN_NAME = 'Origin'
)
BEGIN
    ALTER TABLE dbo.TempProducts ADD Origin NVARCHAR(100) NULL;
    PRINT 'Added Origin column to TempProducts';
END
ELSE
    PRINT 'Origin column already exists in TempProducts';

PRINT 'Origin migration complete.';
