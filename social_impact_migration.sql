-- ============================================================
-- Social Impact Tracker — DB Migration
-- Adds CommissionRate column to FeatureCategories table and
-- seeds the initial commission rates for all central categories.
-- Run once against BoulevardDb.
-- ============================================================

-- 1. Add CommissionRate column (nullable decimal, 2 decimal places)
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME   = 'FeatureCategories'
      AND COLUMN_NAME  = 'CommissionRate'
)
BEGIN
    ALTER TABLE FeatureCategories
        ADD CommissionRate DECIMAL(5, 2) NULL;
    PRINT 'Column CommissionRate added.';
END
ELSE
BEGIN
    PRINT 'Column CommissionRate already exists — skipped.';
END
GO

-- 2. Seed commission rates by FeatureCategoryKey
--    Matches the rates defined in Social Impact Tracker requirements.

-- Grocery (3%)
UPDATE FeatureCategories
SET    CommissionRate = 3.00
WHERE  FeatureCategoryKey = '3b317e3f-cb2f-4fdd-b9c8-3f2186695771'
  AND  (CommissionRate IS NULL OR CommissionRate = 0);

-- Restaurants (15%)
UPDATE FeatureCategories
SET    CommissionRate = 15.00
WHERE  FeatureCategoryKey = 'E7B3A1C2-D4F5-4A6B-8C9D-1E2F3A4B5C6D'
  AND  (CommissionRate IS NULL OR CommissionRate = 0);

-- Retail (7%)
UPDATE FeatureCategories
SET    CommissionRate = 7.00
WHERE  FeatureCategoryKey = 'F1A2B3C4-D5E6-4F70-8B9C-0D1E2F3A4B5C'
  AND  (CommissionRate IS NULL OR CommissionRate = 0);

-- Desserts & Flowers (15%)
UPDATE FeatureCategories
SET    CommissionRate = 15.00
WHERE  FeatureCategoryKey = '88d5d23e-470f-409a-bb6b-def7ab1346fa'
  AND  (CommissionRate IS NULL OR CommissionRate = 0);

-- Typing Services (10%)
UPDATE FeatureCategories
SET    CommissionRate = 10.00
WHERE  FeatureCategoryKey = 'f4309df5-9121-41ad-831a-994c46b62766'
  AND  (CommissionRate IS NULL OR CommissionRate = 0);

-- Insurance (5%)
UPDATE FeatureCategories
SET    CommissionRate = 5.00
WHERE  FeatureCategoryKey = 'c286a46b-5b9a-4519-bb10-8d47ec254ffb'
  AND  (CommissionRate IS NULL OR CommissionRate = 0);

-- Medical (15%)
UPDATE FeatureCategories
SET    CommissionRate = 15.00
WHERE  FeatureCategoryKey = 'bbc98e2d-941b-44c6-8122-0e12a2645b87'
  AND  (CommissionRate IS NULL OR CommissionRate = 0);

-- Beauty / Salon (15%)
UPDATE FeatureCategories
SET    CommissionRate = 15.00
WHERE  FeatureCategoryKey = '25d8c418-2d26-4159-9d7f-970e3b933b42'
  AND  (CommissionRate IS NULL OR CommissionRate = 0);

-- Motors (7%)
UPDATE FeatureCategories
SET    CommissionRate = 7.00
WHERE  FeatureCategoryKey = 'b3e3e680-c8ef-4ab2-a4ac-d75bb48a3647'
  AND  (CommissionRate IS NULL OR CommissionRate = 0);

-- Real Estate (5%)
UPDATE FeatureCategories
SET    CommissionRate = 5.00
WHERE  FeatureCategoryKey = 'DD501B2D-FE22-4C31-B340-1B4237FAB5CC'
  AND  (CommissionRate IS NULL OR CommissionRate = 0);

-- Laundry (default 0 — not specified, can be set in admin panel)
UPDATE FeatureCategories
SET    CommissionRate = 0.00
WHERE  FeatureCategoryKey = '3c4d5e6f-7a8b-9c0d-ef12-345678901abc'
  AND  CommissionRate IS NULL;

-- Photography (default 0 — not specified)
UPDATE FeatureCategories
SET    CommissionRate = 0.00
WHERE  FeatureCategoryKey = '4d5e6f7a-8b9c-0d1e-f234-5678901abcde'
  AND  CommissionRate IS NULL;

-- Travel / Umrah Plan (3%)
-- If a FeatureCategory with name like 'Travel' or 'Umrah' exists update it too.
UPDATE FeatureCategories
SET    CommissionRate = 3.00
WHERE  (Name LIKE '%Travel%' OR Name LIKE '%Umrah%')
  AND  (CommissionRate IS NULL OR CommissionRate = 0);

GO

PRINT 'Social Impact Tracker migration complete.';
