-- ============================================================
-- Seed FeatureCategories + default Services for Laundry & Photography
-- Run ONCE against your DB before using these sections in the admin.
--
-- Laundry     GUID : 3c4d5e6f-7a8b-9c0d-ef12-345678901abc
-- Photography GUID : 4d5e6f7a-8b9c-0d1e-f234-5678901abcde
-- ============================================================

-- ── STEP 1 : FeatureCategories ──────────────────────────────

IF NOT EXISTS (
    SELECT 1 FROM dbo.FeatureCategories
    WHERE FeatureCategoryKey = '3c4d5e6f-7a8b-9c0d-ef12-345678901abc'
)
BEGIN
    INSERT INTO dbo.FeatureCategories
        (FeatureCategoryKey, Name, NameAr, Image, IsActive, IsDelete, FeatureType,
         IsWaitForApproval, IsQuoteEnable, DeliveryCharge, ServiceFee)
    VALUES
        ('3c4d5e6f-7a8b-9c0d-ef12-345678901abc',
         'Laundry', N'غسيل', NULL, 1, 0, 'Service', 0, 0, 0.0, 0.0);
    PRINT 'Laundry FeatureCategory inserted.';
END
ELSE
    PRINT 'Laundry FeatureCategory already exists – skipped.';

IF NOT EXISTS (
    SELECT 1 FROM dbo.FeatureCategories
    WHERE FeatureCategoryKey = '4d5e6f7a-8b9c-0d1e-f234-5678901abcde'
)
BEGIN
    INSERT INTO dbo.FeatureCategories
        (FeatureCategoryKey, Name, NameAr, Image, IsActive, IsDelete, FeatureType,
         IsWaitForApproval, IsQuoteEnable, DeliveryCharge, ServiceFee)
    VALUES
        ('4d5e6f7a-8b9c-0d1e-f234-5678901abcde',
         'Photography', N'تصوير', NULL, 1, 0, 'Service', 0, 0, 0.0, 0.0);
    PRINT 'Photography FeatureCategory inserted.';
END
ELSE
    PRINT 'Photography FeatureCategory already exists – skipped.';

-- ── STEP 2 : Default Services ───────────────────────────────
-- A ServiceType MUST reference a Service, so each FeatureCategory
-- needs at least one Service row. These act as the "provider" entry.

DECLARE @LaundryFCId   INT = (SELECT FeatureCategoryId FROM dbo.FeatureCategories
                               WHERE FeatureCategoryKey = '3c4d5e6f-7a8b-9c0d-ef12-345678901abc');
DECLARE @PhotographyFCId INT = (SELECT FeatureCategoryId FROM dbo.FeatureCategories
                                 WHERE FeatureCategoryKey = '4d5e6f7a-8b9c-0d1e-f234-5678901abcde');

IF @LaundryFCId IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM dbo.Services WHERE FeatureCategoryId = @LaundryFCId AND ParentId = 0 AND IsPackage = 0)
BEGIN
    INSERT INTO dbo.Services
        (ServiceKey, Name, NameAr, Description, DescriptionAr,
         ServiceHour, ServiceMin, FeatureCategoryId,
         Ratings, CheckInTime, CheckOutTime,
         IsPackage, Price, DistanceInKM, ParentId,
         Status, CreateBy, CreateDate)
    VALUES
        (NEWID(),
         'Laundry Service', N'خدمة الغسيل',
         'Default laundry service provider', N'مزود خدمة الغسيل الافتراضي',
         0, 0, @LaundryFCId,
         0, '00:00:00', '00:00:00',
         0, 0, 0, 0,
         'Active', 1, GETDATE());
    PRINT 'Laundry default Service inserted.';
END
ELSE
    PRINT 'Laundry Service already exists or FeatureCategory not found – skipped.';

IF @PhotographyFCId IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM dbo.Services WHERE FeatureCategoryId = @PhotographyFCId AND ParentId = 0 AND IsPackage = 0)
BEGIN
    INSERT INTO dbo.Services
        (ServiceKey, Name, NameAr, Description, DescriptionAr,
         ServiceHour, ServiceMin, FeatureCategoryId,
         Ratings, CheckInTime, CheckOutTime,
         IsPackage, Price, DistanceInKM, ParentId,
         Status, CreateBy, CreateDate)
    VALUES
        (NEWID(),
         'Photography Service', N'خدمة التصوير',
         'Default photography service provider', N'مزود خدمة التصوير الافتراضي',
         0, 0, @PhotographyFCId,
         0, '00:00:00', '00:00:00',
         0, 0, 0, 0,
         'Active', 1, GETDATE());
    PRINT 'Photography default Service inserted.';
END
ELSE
    PRINT 'Photography Service already exists or FeatureCategory not found – skipped.';

-- ── STEP 3 : Verify ─────────────────────────────────────────
SELECT fc.FeatureCategoryId, fc.FeatureCategoryKey, fc.Name AS CategoryName,
       s.ServiceId, s.Name AS ServiceName, s.Status
FROM dbo.FeatureCategories fc
LEFT JOIN dbo.Services s ON s.FeatureCategoryId = fc.FeatureCategoryId
                         AND s.ParentId = 0 AND s.IsPackage = 0
WHERE fc.FeatureCategoryKey IN (
    '3c4d5e6f-7a8b-9c0d-ef12-345678901abc',
    '4d5e6f7a-8b9c-0d1e-f234-5678901abcde'
)
ORDER BY fc.FeatureCategoryId;
