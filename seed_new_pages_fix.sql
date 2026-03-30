-- Fix the 3 failed inserts
SET NOCOUNT ON;

-- Valid FeatureCategoryIds: 1,2,3,4,5,6,7,8,9,11,12,13,16,17,18 (15 values)
-- Map via CHOOSE on (rn % 15) + 1

------------------------------------------------------------
-- Fix Brands (50,000)
------------------------------------------------------------
PRINT 'Seeding Brands (fix)...';
;WITH n AS (
    SELECT TOP 50000 ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS rn
    FROM sys.all_columns a CROSS JOIN sys.all_columns b
)
INSERT INTO Brands
    (BrandKey, Title, TitleAr, Details, DetailsAr, IsFeature, IsTrenbding,
     FeatureCategoryId, Status, CreateDate, CreateBy)
SELECT
    NEWID(),
    'Brand ' + CAST(rn AS VARCHAR(10)),
    N'ماركة ' + CAST(rn AS VARCHAR(10)),
    'Brand details for item ' + CAST(rn AS VARCHAR(10)),
    N'تفاصيل الماركة ' + CAST(rn AS VARCHAR(10)),
    CASE WHEN rn % 4 = 0 THEN 1 ELSE 0 END,
    CASE WHEN rn % 6 = 0 THEN 1 ELSE 0 END,
    CHOOSE((rn % 15) + 1, 1,2,3,4,5,6,7,8,9,11,12,13,16,17,18),
    'Active',
    DATEADD(SECOND, -rn * 60, GETDATE()),
    1
FROM n;
PRINT 'Brands done.';

------------------------------------------------------------
-- Fix OfferInformations (50,000)
------------------------------------------------------------
PRINT 'Seeding OfferInformations (fix)...';
;WITH n AS (
    SELECT TOP 50000 ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS rn
    FROM sys.all_columns a CROSS JOIN sys.all_columns b
)
INSERT INTO OfferInformations
    (OfferInformationKey, Title, TitleAr, Description, DescriptionAr,
     FeatureCategoryId, IsBrand, IsCategory, IsProduct, IsService, IsTimeLimit, IsTrending,
     FeatureType, FeatureTypeAr,
     Status, CreateDate, CreateBy)
SELECT
    NEWID(),
    'Offer ' + CAST(rn AS VARCHAR(10)),
    N'عرض ' + CAST(rn AS VARCHAR(10)),
    'Description for offer ' + CAST(rn AS VARCHAR(10)),
    N'وصف العرض ' + CAST(rn AS VARCHAR(10)),
    CHOOSE((rn % 15) + 1, 1,2,3,4,5,6,7,8,9,11,12,13,16,17,18),
    CASE WHEN rn % 5 = 0 THEN 1 ELSE 0 END,
    CASE WHEN rn % 6 = 0 THEN 1 ELSE 0 END,
    CASE WHEN rn % 4 = 0 THEN 1 ELSE 0 END,
    CASE WHEN rn % 3 = 0 THEN 1 ELSE 0 END,
    CASE WHEN rn % 7 = 0 THEN 1 ELSE 0 END,
    CASE WHEN rn % 8 = 0 THEN 1 ELSE 0 END,
    CASE rn % 3 WHEN 0 THEN 'Hotel' WHEN 1 THEN 'Grocery' ELSE 'Motors' END,
    CASE rn % 3 WHEN 0 THEN N'فندق' WHEN 1 THEN N'بقالة' ELSE N'سيارات' END,
    'Active',
    DATEADD(SECOND, -rn * 60, GETDATE()),
    1
FROM n;
PRINT 'OfferInformations done.';

------------------------------------------------------------
-- Fix Child Services (50,000)
-- Non-nullable: ServiceHour, FeatureCategoryId, CreateBy, CreateDate,
--               ServiceKey, Ratings, CheckInTime, CheckOutTime,
--               ServiceMin, IsPackage, Price, DistanceInKM, ParentId
------------------------------------------------------------
PRINT 'Seeding Child Services (fix)...';
;WITH n AS (
    SELECT TOP 50000 ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS rn
    FROM sys.all_columns a CROSS JOIN sys.all_columns b
)
INSERT INTO Services
    (ServiceKey, Name, NameAr, Description, DescriptionAr,
     ServiceHour, ServiceMin, FeatureCategoryId,
     Ratings, CheckInTime, CheckOutTime,
     IsPackage, Price, DistanceInKM,
     ParentId, Status, CreateDate, CreateBy)
SELECT
    NEWID(),
    'Sub Service ' + CAST(rn AS VARCHAR(10)),
    N'خدمة فرعية ' + CAST(rn AS VARCHAR(10)),
    'Description for sub service ' + CAST(rn AS VARCHAR(10)),
    N'وصف الخدمة الفرعية ' + CAST(rn AS VARCHAR(10)),
    (rn % 8) + 1,
    (rn % 60),
    CHOOSE((rn % 15) + 1, 1,2,3,4,5,6,7,8,9,11,12,13,16,17,18),
    0.0,
    CAST('08:00:00' AS TIME),
    CAST('20:00:00' AS TIME),
    0,
    CAST((rn % 500) + 50 AS DECIMAL(18,2)),
    CAST((rn % 50) AS DECIMAL(18,2)),
    CASE rn % 5 WHEN 0 THEN 15 WHEN 1 THEN 16 WHEN 2 THEN 17 WHEN 3 THEN 18 ELSE 19 END,
    'Active',
    DATEADD(SECOND, -rn * 60, GETDATE()),
    1
FROM n;
PRINT 'Child Services done.';

PRINT 'Fix seeding complete!';
