-- ============================================================
-- Fix: Remove duplicate "Resturants" FeatureCategory
-- The correct entry is keyed 'E7B3A1C2-D4F5-4A6B-8C9D-1E2F3A4B5C6D'
-- The old duplicate has a typo in the name and 0% commission.
-- Run ONCE against BoulevardDb.
-- ============================================================

-- 1. Show both entries before the fix
SELECT FeatureCategoryId, FeatureCategoryKey, Name, NameAr, CommissionRate, IsDelete
FROM dbo.FeatureCategories
WHERE Name IN ('Restaurant', 'Resturants')
   OR NameAr IN (N'مطعم', N'المطاعم');

GO

-- 2. Soft-delete the duplicate (any Restaurant entry that is NOT the canonical key)
UPDATE dbo.FeatureCategories
SET    IsDelete = 1
WHERE  (Name = 'Resturants' OR NameAr = N'المطاعم')
  AND  FeatureCategoryKey <> 'E7B3A1C2-D4F5-4A6B-8C9D-1E2F3A4B5C6D';

PRINT 'Fixed: ' + CAST(@@ROWCOUNT AS VARCHAR) + ' duplicate row(s) soft-deleted.';

GO

-- 3. Confirm only the correct entry remains visible
SELECT FeatureCategoryId, FeatureCategoryKey, Name, NameAr, CommissionRate, IsDelete
FROM dbo.FeatureCategories
WHERE Name IN ('Restaurant', 'Resturants')
   OR NameAr IN (N'مطعم', N'المطاعم');
