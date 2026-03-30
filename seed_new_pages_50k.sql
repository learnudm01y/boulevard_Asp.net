-- ============================================================
-- Seed 50,000 rows for each of the 9 newly paginated tables
-- Run against BoulevardDb on .\SQLEXPRESS
-- ============================================================
SET NOCOUNT ON;

-- Helper: parent service IDs (1,2,3,15,16,17,18,19)
-- FeatureCategory IDs: 1..18
-- Brand IDs (Motors): 16,17,18,19,21
-- ServiceType IDs (sample): 1,3,14,15,16

------------------------------------------------------------
-- 1. ServiceType  (50,000)
------------------------------------------------------------
PRINT 'Seeding ServiceTypes...';
;WITH n AS (
    SELECT TOP 50000 ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS rn
    FROM sys.all_columns a CROSS JOIN sys.all_columns b
)
INSERT INTO ServiceTypes
    (ServiceTypeKey, ServiceTypeName, ServiceTypeNameAr, PersoneQuantity, AdultQuantity,
     ChildrenQuantity, ServiceHour, ServiceMin, Size, SizeAr, Price, PaymentType, IsPackage,
     ServiceId, Status, CreateDate, CreateBy)
SELECT
    NEWID(),
    'Service Type ' + CAST(rn AS VARCHAR(10)),
    N'نوع الخدمة ' + CAST(rn AS VARCHAR(10)),
    (rn % 5) + 1,
    (rn % 3) + 1,
    rn % 3,
    (rn % 8) + 1,
    (rn % 60),
    CAST((rn % 200) + 10 AS VARCHAR(10)) + ' sqm',
    CAST((rn % 200) + 10 AS VARCHAR(10)) + N' م²',
    CAST((rn % 500) + 50 AS FLOAT) * 1.0,
    CASE rn % 3 WHEN 0 THEN 'Cash' WHEN 1 THEN 'Card' ELSE 'Online' END,
    CASE WHEN rn % 5 = 0 THEN 1 ELSE 0 END,
    CASE rn % 8
        WHEN 0 THEN 1  WHEN 1 THEN 2  WHEN 2 THEN 3
        WHEN 3 THEN 15 WHEN 4 THEN 16 WHEN 5 THEN 17
        WHEN 6 THEN 18 ELSE 19 END,
    'Active',
    DATEADD(SECOND, -rn * 60, GETDATE()),
    1
FROM n;
PRINT 'ServiceTypes done.';

------------------------------------------------------------
-- 2. ServiceLandmark  (50,000)
------------------------------------------------------------
PRINT 'Seeding ServiceLandmarks...';
;WITH n AS (
    SELECT TOP 50000 ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS rn
    FROM sys.all_columns a CROSS JOIN sys.all_columns b
)
INSERT INTO ServiceLandmarks
    (ServiceLandmarkKey, Name, NameAr, Address, Distance, Latitude, Longitude,
     ServiceId, Status, CreateDate, CreateBy)
SELECT
    NEWID(),
    'Landmark ' + CAST(rn AS VARCHAR(10)),
    N'معلم ' + CAST(rn AS VARCHAR(10)),
    'Address ' + CAST(rn AS VARCHAR(10)) + ', City',
    CAST((rn % 50) + 1 AS FLOAT) * 0.5,
    CAST(24.0 + (rn % 100) * 0.005 AS VARCHAR(20)),
    CAST(54.0 + (rn % 100) * 0.005 AS VARCHAR(20)),
    CASE rn % 8
        WHEN 0 THEN 1  WHEN 1 THEN 2  WHEN 2 THEN 3
        WHEN 3 THEN 15 WHEN 4 THEN 16 WHEN 5 THEN 17
        WHEN 6 THEN 18 ELSE 19 END,
    'Active',
    DATEADD(SECOND, -rn * 60, GETDATE()),
    1
FROM n;
PRINT 'ServiceLandmarks done.';

------------------------------------------------------------
-- 3. Brands  (50,000)
------------------------------------------------------------
PRINT 'Seeding Brands...';
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
    (rn % 18) + 1,
    'Active',
    DATEADD(SECOND, -rn * 60, GETDATE()),
    1
FROM n;
PRINT 'Brands done.';

------------------------------------------------------------
-- 4. VehicalModels  (50,000)
------------------------------------------------------------
PRINT 'Seeding VehicalModels...';
;WITH n AS (
    SELECT TOP 50000 ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS rn
    FROM sys.all_columns a CROSS JOIN sys.all_columns b
)
INSERT INTO VehicalModels
    (VehicalModelKey, VehicalModelName, VehicalModelNameAr, ModelDetails, ModelDetailsAr,
     BrandId, Status, CreateDate, CreateBy)
SELECT
    NEWID(),
    'Vehicle Model ' + CAST(rn AS VARCHAR(10)),
    N'موديل ' + CAST(rn AS VARCHAR(10)),
    'Details for vehicle model ' + CAST(rn AS VARCHAR(10)),
    N'تفاصيل الموديل ' + CAST(rn AS VARCHAR(10)),
    CASE rn % 5 WHEN 0 THEN 16 WHEN 1 THEN 17 WHEN 2 THEN 18 WHEN 3 THEN 19 ELSE 21 END,
    'Active',
    DATEADD(SECOND, -rn * 60, GETDATE()),
    1
FROM n;
PRINT 'VehicalModels done.';

------------------------------------------------------------
-- 5. FaqServices  (50,000)
------------------------------------------------------------
PRINT 'Seeding FaqServices...';
;WITH n AS (
    SELECT TOP 50000 ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS rn
    FROM sys.all_columns a CROSS JOIN sys.all_columns b
)
INSERT INTO FaqServices
    (FAQKey, FaqTitle, FaqDescription, FaqTitleAr, FaqDescriptionAr,
     FeatureType, FeatureTypeId, IsActive, LastUpdate, Status, CreateDate, CreateBy)
SELECT
    NEWID(),
    'FAQ ' + CAST(rn AS VARCHAR(10)),
    'Answer to FAQ number ' + CAST(rn AS VARCHAR(10)),
    N'سؤال ' + CAST(rn AS VARCHAR(10)),
    N'إجابة السؤال رقم ' + CAST(rn AS VARCHAR(10)),
    CASE rn % 3 WHEN 0 THEN 'Hotel' WHEN 1 THEN 'Service' ELSE 'General' END,
    CASE rn % 8
        WHEN 0 THEN 1  WHEN 1 THEN 2  WHEN 2 THEN 3
        WHEN 3 THEN 15 WHEN 4 THEN 16 WHEN 5 THEN 17
        WHEN 6 THEN 18 ELSE 19 END,
    1,
    DATEADD(SECOND, -rn * 60, GETDATE()),
    'Active',
    DATEADD(SECOND, -rn * 60, GETDATE()),
    1
FROM n;
PRINT 'FaqServices done.';

------------------------------------------------------------
-- 6. PropertyInformations  (50,000)
------------------------------------------------------------
PRINT 'Seeding PropertyInformations...';
;WITH n AS (
    SELECT TOP 50000 ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS rn
    FROM sys.all_columns a CROSS JOIN sys.all_columns b
)
INSERT INTO PropertyInformations
    (PropertyInfoKey, Type, TypeAr, Purpose, PurposeAr, RefNo, Furnishing, FurnishingAr,
     PropertyPhoneNo, PropertyEmail,
     Status, CreateDate, CreateBy)
SELECT
    NEWID(),
    CASE rn % 4 WHEN 0 THEN 'Apartment' WHEN 1 THEN 'Villa' WHEN 2 THEN 'Studio' ELSE 'Penthouse' END,
    CASE rn % 4 WHEN 0 THEN N'شقة' WHEN 1 THEN N'فيلا' WHEN 2 THEN N'استوديو' ELSE N'بنتهاوس' END,
    CASE rn % 3 WHEN 0 THEN 'Sale' WHEN 1 THEN 'Rent' ELSE 'Both' END,
    CASE rn % 3 WHEN 0 THEN N'بيع' WHEN 1 THEN N'إيجار' ELSE N'كلاهما' END,
    'REF-' + RIGHT('000000' + CAST(rn AS VARCHAR(10)), 6),
    CASE rn % 3 WHEN 0 THEN 'Furnished' WHEN 1 THEN 'Unfurnished' ELSE 'Semi-furnished' END,
    CASE rn % 3 WHEN 0 THEN N'مفروش' WHEN 1 THEN N'غير مفروش' ELSE N'نصف مفروش' END,
    '+971 50 ' + RIGHT('0000000' + CAST(1000000 + rn AS VARCHAR(10)), 7),
    'property' + CAST(rn AS VARCHAR(10)) + '@test.com',
    'Active',
    DATEADD(SECOND, -rn * 60, GETDATE()),
    1
FROM n;
PRINT 'PropertyInformations done.';

------------------------------------------------------------
-- 7. OfferInformations  (50,000)
------------------------------------------------------------
PRINT 'Seeding OfferInformations...';
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
    (rn % 18) + 1,
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
-- 8. ServiceTypeAmenities  (50,000)
--    Use existing ServiceType IDs (1,3,14,15,16)
------------------------------------------------------------
PRINT 'Seeding ServiceTypeAmenities...';
;WITH n AS (
    SELECT TOP 50000 ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS rn
    FROM sys.all_columns a CROSS JOIN sys.all_columns b
)
INSERT INTO ServiceTypeAmenities
    (ServiceAmenityKey, ServiceTypeId, AmenitiesName, AmenitiesNameAr,
     AmenitiesLogo, AmenitiesType, LinkedWithFile,
     Status, CreateDate, CreateBy)
SELECT
    NEWID(),
    CASE rn % 5 WHEN 0 THEN 1 WHEN 1 THEN 3 WHEN 2 THEN 14 WHEN 3 THEN 15 ELSE 16 END,
    'Amenity ' + CAST(rn AS VARCHAR(10)),
    N'ميزة ' + CAST(rn AS VARCHAR(10)),
    NULL,
    CASE rn % 4 WHEN 0 THEN 'WiFi' WHEN 1 THEN 'Parking' WHEN 2 THEN 'Pool' ELSE 'Gym' END,
    0,
    'Active',
    DATEADD(SECOND, -rn * 60, GETDATE()),
    1
FROM n;
PRINT 'ServiceTypeAmenities done.';

------------------------------------------------------------
-- 9. Child Services (Services with ParentId > 0) (50,000)
--    ParentId cycles through 15,16,17,18,19
------------------------------------------------------------
PRINT 'Seeding Child Services...';
;WITH n AS (
    SELECT TOP 50000 ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS rn
    FROM sys.all_columns a CROSS JOIN sys.all_columns b
)
INSERT INTO Services
    (ServiceKey, Name, NameAr, Description, DescriptionAr,
     ParentId, Status, CreateDate, CreateBy)
SELECT
    NEWID(),
    'Sub Service ' + CAST(rn AS VARCHAR(10)),
    N'خدمة فرعية ' + CAST(rn AS VARCHAR(10)),
    'Description for sub service ' + CAST(rn AS VARCHAR(10)),
    N'وصف الخدمة الفرعية ' + CAST(rn AS VARCHAR(10)),
    CASE rn % 5 WHEN 0 THEN 15 WHEN 1 THEN 16 WHEN 2 THEN 17 WHEN 3 THEN 18 ELSE 19 END,
    'Active',
    DATEADD(SECOND, -rn * 60, GETDATE()),
    1
FROM n;
PRINT 'Child Services done.';

PRINT 'All seeding complete!';
