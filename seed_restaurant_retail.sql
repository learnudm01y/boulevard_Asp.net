-- ============================================================
-- Seed: Restaurant & Retail FeatureCategories
-- Run ONCE against production/dev DB before deploying code.
-- ============================================================

-- Fixed GUIDs (hardcoded in API controllers – do NOT change after first run)
-- Restaurant : E7B3A1C2-D4F5-4A6B-8C9D-1E2F3A4B5C6D
-- Retail     : F1A2B3C4-D5E6-4F70-8B9C-0D1E2F3A4B5C

-- Safety: skip if already inserted
IF NOT EXISTS (
    SELECT 1 FROM dbo.FeatureCategories
    WHERE FeatureCategoryKey = 'E7B3A1C2-D4F5-4A6B-8C9D-1E2F3A4B5C6D'
)
BEGIN
    INSERT INTO dbo.FeatureCategories
        (FeatureCategoryKey, Name, NameAr, Image, IsActive, IsDelete, FeatureType,
         IsWaitForApproval, IsQuoteEnable, DeliveryCharge, ServiceFee)
    VALUES
        ('E7B3A1C2-D4F5-4A6B-8C9D-1E2F3A4B5C6D',
         'Restaurant', N'مطعم',
         '',     -- add icon path later via Admin → Feature Category
         1, 0, 'Product',
         0, 0, 0.0, 0.0);
    PRINT 'Restaurant FeatureCategory inserted.';
END
ELSE
    PRINT 'Restaurant already exists – skipped.';

IF NOT EXISTS (
    SELECT 1 FROM dbo.FeatureCategories
    WHERE FeatureCategoryKey = 'F1A2B3C4-D5E6-4F70-8B9C-0D1E2F3A4B5C'
)
BEGIN
    INSERT INTO dbo.FeatureCategories
        (FeatureCategoryKey, Name, NameAr, Image, IsActive, IsDelete, FeatureType,
         IsWaitForApproval, IsQuoteEnable, DeliveryCharge, ServiceFee)
    VALUES
        ('F1A2B3C4-D5E6-4F70-8B9C-0D1E2F3A4B5C',
         'Retail', N'تجزئة',
         '',     -- add icon path later via Admin → Feature Category
         1, 0, 'Product',
         0, 0, 0.0, 0.0);
    PRINT 'Retail FeatureCategory inserted.';
END
ELSE
    PRINT 'Retail already exists – skipped.';

-- Verify result
SELECT FeatureCategoryId, FeatureCategoryKey, Name, NameAr, FeatureType, IsActive
FROM dbo.FeatureCategories
WHERE FeatureCategoryKey IN (
    'E7B3A1C2-D4F5-4A6B-8C9D-1E2F3A4B5C6D',
    'F1A2B3C4-D5E6-4F70-8B9C-0D1E2F3A4B5C'
);
