-- ============================================================
-- Cloud DB Migration Script
-- Run this script on the cloud SQL Server to bring the DB
-- schema up to date with the current codebase.
--
-- Adds columns that were added directly in code/manually
-- without proper EF migration files:
--   TempProducts: MiniCategory, MiniCategoryArabic, IcvBoulevardScore, Origin
--   Products:     MiniCategory, MiniCategoryArabic, IcvBoulevardScore, Origin
-- ============================================================

-- ── TempProducts ─────────────────────────────────────────────

IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME='TempProducts' AND COLUMN_NAME='MiniCategory'
)
    ALTER TABLE dbo.TempProducts ADD MiniCategory NVARCHAR(MAX) NULL;

IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME='TempProducts' AND COLUMN_NAME='MiniCategoryArabic'
)
    ALTER TABLE dbo.TempProducts ADD MiniCategoryArabic NVARCHAR(MAX) NULL;

IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME='TempProducts' AND COLUMN_NAME='IcvBoulevardScore'
)
    ALTER TABLE dbo.TempProducts ADD IcvBoulevardScore NVARCHAR(50) NULL;

IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME='TempProducts' AND COLUMN_NAME='Origin'
)
    ALTER TABLE dbo.TempProducts ADD Origin NVARCHAR(100) NULL;

-- ── Products ─────────────────────────────────────────────────

IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME='Products' AND COLUMN_NAME='MiniCategory'
)
    ALTER TABLE dbo.Products ADD MiniCategory NVARCHAR(MAX) NULL;

IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME='Products' AND COLUMN_NAME='MiniCategoryArabic'
)
    ALTER TABLE dbo.Products ADD MiniCategoryArabic NVARCHAR(MAX) NULL;

IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME='Products' AND COLUMN_NAME='IcvBoulevardScore'
)
    ALTER TABLE dbo.Products ADD IcvBoulevardScore NVARCHAR(50) NULL;

IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME='Products' AND COLUMN_NAME='Origin'
)
    ALTER TABLE dbo.Products ADD Origin NVARCHAR(100) NULL;

-- ── Verification ─────────────────────────────────────────────
SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME IN ('TempProducts','Products')
  AND COLUMN_NAME IN ('MiniCategory','MiniCategoryArabic','IcvBoulevardScore','Origin')
ORDER BY TABLE_NAME, COLUMN_NAME;
