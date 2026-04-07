-- ============================================================
-- Seed: Grocery FeatureCategory
-- Run ONCE against dev/production DB before importing data.
-- GUID is fixed and referenced in docs/grocery-data-flow-problem-report.md
-- ============================================================

-- Fixed GUID for Grocery (do NOT change after first run)
-- Grocery : 3b317e3f-cb2f-4fdd-b9c8-3f2186695771

IF NOT EXISTS (
    SELECT 1 FROM dbo.FeatureCategories
    WHERE FeatureCategoryKey = '3b317e3f-cb2f-4fdd-b9c8-3f2186695771'
)
BEGIN
    INSERT INTO dbo.FeatureCategories
        (FeatureCategoryKey, Name, NameAr, Image, IsActive, IsDelete, FeatureType,
         IsWaitForApproval, IsQuoteEnable, DeliveryCharge, ServiceFee)
    VALUES
        ('3b317e3f-cb2f-4fdd-b9c8-3f2186695771',
         'Grocery', N'بقالة',
         '',     -- add icon path later via Admin → Feature Category
         1, 0, 'Product',
         0, 0, 0.0, 0.0);
    PRINT 'Grocery FeatureCategory inserted.';
END
ELSE
    PRINT 'Grocery already exists – skipped.';

-- Verify result
SELECT FeatureCategoryId, FeatureCategoryKey, Name, NameAr, FeatureType, IsActive
FROM dbo.FeatureCategories
WHERE FeatureCategoryKey = '3b317e3f-cb2f-4fdd-b9c8-3f2186695771';
