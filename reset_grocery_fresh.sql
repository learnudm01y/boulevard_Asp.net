-- ============================================================
-- reset_grocery_fresh.sql
-- 1) Cleans the 3 old test products (NatureFirst/GoldenHarvest/OliveGold)
--    and their messy Excel-imported categories
-- 2) Seeds 3 real sellers with 4 products each (12 products total)
-- Run against BoulevardDb
-- ============================================================
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET NOCOUNT ON;

DECLARE @GroceryFCId INT;
SELECT @GroceryFCId = FeatureCategoryId
FROM dbo.FeatureCategories
WHERE FeatureCategoryKey = '3b317e3f-cb2f-4fdd-b9c8-3f2186695771';

IF @GroceryFCId IS NULL
BEGIN
    RAISERROR('Grocery FeatureCategory not found.', 16, 1);
    RETURN;
END
PRINT 'GroceryFCId = ' + CAST(@GroceryFCId AS VARCHAR(10));

-- ============================================================
-- STEP 1: CLEAN OLD DATA
-- ============================================================

-- 1a. Remove ProductCategories for the 3 old products
DELETE pc
FROM dbo.ProductCategories pc
INNER JOIN dbo.Products p ON p.ProductId = pc.ProductId
WHERE p.FeatureCategoryId = @GroceryFCId
  AND p.ProductKey IN (
      -- keys used by Excel-uploaded products (no CC0-prefix keys)
      -- match by BrandId of the brands we are about to clean
      SELECT ProductKey FROM dbo.Products
      WHERE BrandId IN (102867, 102868, 102869)  -- NatureFirst, GoldenHarvest, OliveGold
  );

PRINT 'Old ProductCategories rows deleted.';

-- 1b. Hard-delete the 3 old products
DELETE FROM dbo.Products
WHERE BrandId IN (102867, 102868, 102869)
  AND FeatureCategoryId = @GroceryFCId;

PRINT 'Old Products deleted.';

-- 1c. Soft-delete the 3 old brands
UPDATE dbo.Brands
SET Status = 'Deleted', DeleteDate = GETDATE(), DeleteBy = 1
WHERE BrandId IN (102867, 102868, 102869)
  AND DeleteDate IS NULL;

PRINT 'Old Brands (NatureFirst, GoldenHarvest, OliveGold) soft-deleted.';

-- 1d. Clean up the fragmented Excel-imported categories (2334–2345)
--     These are sub-tags like "Milk", "Full Fat", "Basmati", "Long Grain" etc.
DELETE FROM dbo.ProductCategories
WHERE CategoryId BETWEEN 2334 AND 2345;

DELETE FROM dbo.Categories
WHERE CategoryId BETWEEN 2334 AND 2345
  AND FeatureCategoryId = @GroceryFCId;

PRINT 'Old fragmented Excel categories deleted.';

-- ============================================================
-- STEP 2: INSERT BRANDS (SELLERS)
-- ============================================================

IF NOT EXISTS (SELECT 1 FROM dbo.Brands WHERE BrandKey = 'AA000001-0001-0001-0001-000000000001')
BEGIN
    INSERT INTO dbo.Brands
        (BrandKey, Title, TitleAr, Details, DetailsAr,
         IsFeature, IsTrenbding, IsDeliveryEnabled,
         FeatureCategoryId, Status, CreateDate, CreateBy)
    VALUES
        ('AA000001-0001-0001-0001-000000000001',
         'GreenGrocer', N'جرين جروسر',
         'Fresh dairy, eggs and cheese delivered daily.',
         N'منتجات الألبان والبيض والجبن الطازجة توصيل يومي.',
         1, 0, 1, @GroceryFCId, 'Active', GETDATE(), 1);
    PRINT 'Brand GreenGrocer inserted.';
END ELSE PRINT 'Brand GreenGrocer already exists.';

IF NOT EXISTS (SELECT 1 FROM dbo.Brands WHERE BrandKey = 'AA000002-0002-0002-0002-000000000002')
BEGIN
    INSERT INTO dbo.Brands
        (BrandKey, Title, TitleAr, Details, DetailsAr,
         IsFeature, IsTrenbding, IsDeliveryEnabled,
         FeatureCategoryId, Status, CreateDate, CreateBy)
    VALUES
        ('AA000002-0002-0002-0002-000000000002',
         'FreshFarm', N'فريش فارم',
         'Premium grains, oils and pantry staples.',
         N'حبوب وزيوت ومواد أساسية فاخرة للمطبخ.',
         1, 0, 1, @GroceryFCId, 'Active', GETDATE(), 1);
    PRINT 'Brand FreshFarm inserted.';
END ELSE PRINT 'Brand FreshFarm already exists.';

IF NOT EXISTS (SELECT 1 FROM dbo.Brands WHERE BrandKey = 'AA000003-0003-0003-0003-000000000003')
BEGIN
    INSERT INTO dbo.Brands
        (BrandKey, Title, TitleAr, Details, DetailsAr,
         IsFeature, IsTrenbding, IsDeliveryEnabled,
         FeatureCategoryId, Status, CreateDate, CreateBy)
    VALUES
        ('AA000003-0003-0003-0003-000000000003',
         'OrganicWorld', N'عالم العضوي',
         'Certified organic fruits and vegetables.',
         N'فواكه وخضروات عضوية معتمدة.',
         1, 1, 1, @GroceryFCId, 'Active', GETDATE(), 1);
    PRINT 'Brand OrganicWorld inserted.';
END ELSE PRINT 'Brand OrganicWorld already exists.';

DECLARE @BrandIdGreenGrocer  INT,
        @BrandIdFreshFarm    INT,
        @BrandIdOrganicWorld INT;

SELECT @BrandIdGreenGrocer  = BrandId FROM dbo.Brands WHERE BrandKey = 'AA000001-0001-0001-0001-000000000001';
SELECT @BrandIdFreshFarm    = BrandId FROM dbo.Brands WHERE BrandKey = 'AA000002-0002-0002-0002-000000000002';
SELECT @BrandIdOrganicWorld = BrandId FROM dbo.Brands WHERE BrandKey = 'AA000003-0003-0003-0003-000000000003';

PRINT 'BrandIdGreenGrocer='  + CAST(@BrandIdGreenGrocer  AS VARCHAR(10));
PRINT 'BrandIdFreshFarm='    + CAST(@BrandIdFreshFarm    AS VARCHAR(10));
PRINT 'BrandIdOrganicWorld=' + CAST(@BrandIdOrganicWorld AS VARCHAR(10));

-- ============================================================
-- STEP 3: INSERT CATEGORIES
-- ============================================================

IF NOT EXISTS (SELECT 1 FROM dbo.Categories WHERE CategoryKey = 'BB000001-0001-0001-0001-000000000001')
BEGIN
    INSERT INTO dbo.Categories
        (CategoryKey, CategoryName, CategoryDescription, Image, Icon,
         ParentId, FeatureCategoryId, IsTop, IsTrenbding, IsPackagecategory,
         label, Status, CreateDate, CreateBy)
    VALUES
        ('BB000001-0001-0001-0001-000000000001',
         'Dairy and Eggs', N'ألبان وبيض', '', '',
         NULL, @GroceryFCId, 1, 0, 0, 1, 'Active', GETDATE(), 1);
    PRINT 'Category Dairy and Eggs inserted.';
END ELSE PRINT 'Category Dairy and Eggs already exists.';

IF NOT EXISTS (SELECT 1 FROM dbo.Categories WHERE CategoryKey = 'BB000002-0002-0002-0002-000000000002')
BEGIN
    INSERT INTO dbo.Categories
        (CategoryKey, CategoryName, CategoryDescription, Image, Icon,
         ParentId, FeatureCategoryId, IsTop, IsTrenbding, IsPackagecategory,
         label, Status, CreateDate, CreateBy)
    VALUES
        ('BB000002-0002-0002-0002-000000000002',
         'Grains and Pantry', N'حبوب ومؤن', '', '',
         NULL, @GroceryFCId, 1, 0, 0, 2, 'Active', GETDATE(), 1);
    PRINT 'Category Grains and Pantry inserted.';
END ELSE PRINT 'Category Grains and Pantry already exists.';

IF NOT EXISTS (SELECT 1 FROM dbo.Categories WHERE CategoryKey = 'BB000003-0003-0003-0003-000000000003')
BEGIN
    INSERT INTO dbo.Categories
        (CategoryKey, CategoryName, CategoryDescription, Image, Icon,
         ParentId, FeatureCategoryId, IsTop, IsTrenbding, IsPackagecategory,
         label, Status, CreateDate, CreateBy)
    VALUES
        ('BB000003-0003-0003-0003-000000000003',
         'Fresh Produce', N'منتجات طازجة', '', '',
         NULL, @GroceryFCId, 1, 1, 0, 3, 'Active', GETDATE(), 1);
    PRINT 'Category Fresh Produce inserted.';
END ELSE PRINT 'Category Fresh Produce already exists.';

DECLARE @CatIdDairy   INT,
        @CatIdGrains  INT,
        @CatIdProduce INT;

SELECT @CatIdDairy   = CategoryId FROM dbo.Categories WHERE CategoryKey = 'BB000001-0001-0001-0001-000000000001';
SELECT @CatIdGrains  = CategoryId FROM dbo.Categories WHERE CategoryKey = 'BB000002-0002-0002-0002-000000000002';
SELECT @CatIdProduce = CategoryId FROM dbo.Categories WHERE CategoryKey = 'BB000003-0003-0003-0003-000000000003';

-- ============================================================
-- STEP 4: INSERT PRODUCTS – GreenGrocer (Dairy & Eggs)
-- ============================================================

IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE ProductKey = 'CC010001-0001-0001-0001-000000000001')
    INSERT INTO dbo.Products
        (ProductKey, ProductName, ProductNameAr, ProductSlag, ProductSlagAr,
         ProductDescription, ProductDescriptionAr, DeliveryInfo, DeliveryInfoArabic,
         AttributeCode, AttributeName, AttributeNameArabic, Barcode,
         ProductPrice, BrandId, FeatureCategoryId, StockQuantity,
         IsScheduled, ProductType, AvgRatings, Status, CreateDate, CreateBy)
    VALUES ('CC010001-0001-0001-0001-000000000001',
        'Fresh Whole Milk 1L', N'حليب كامل الدسم طازج 1 لتر',
        'fresh-whole-milk-1l', N'حليب-كامل-الدسم-1-لتر',
        'Fresh full-fat milk, rich in calcium and vitamins.',
        N'حليب طازج كامل الدسم غني بالكالسيوم والفيتامينات.',
        'Delivered within 1-2 business days', N'يتم التوصيل خلال 1-2 يوم عمل',
        '1L', '1 Liter', N'1 لتر', 'MLK-WHL-1L',
        3.50, @BrandIdGreenGrocer, @GroceryFCId, 500,
        0, 1, 0, 'Active', GETDATE(), 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE ProductKey = 'CC010002-0001-0001-0001-000000000002')
    INSERT INTO dbo.Products
        (ProductKey, ProductName, ProductNameAr, ProductSlag, ProductSlagAr,
         ProductDescription, ProductDescriptionAr, DeliveryInfo, DeliveryInfoArabic,
         AttributeCode, AttributeName, AttributeNameArabic, Barcode,
         ProductPrice, BrandId, FeatureCategoryId, StockQuantity,
         IsScheduled, ProductType, AvgRatings, Status, CreateDate, CreateBy)
    VALUES ('CC010002-0001-0001-0001-000000000002',
        'Free Range Eggs 12pc', N'بيض حر 12 قطعة',
        'free-range-eggs-12pc', N'بيض-حر-12-قطعة',
        'Farm fresh free-range eggs, pack of 12.',
        N'بيض طازج من مزارع الدجاج الحر، عبوة 12 قطعة.',
        'Delivered within 1-2 business days', N'يتم التوصيل خلال 1-2 يوم عمل',
        '12PC', '12 Pieces', N'12 قطعة', 'EGG-FR-12PC',
        6.50, @BrandIdGreenGrocer, @GroceryFCId, 300,
        0, 1, 0, 'Active', GETDATE(), 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE ProductKey = 'CC010003-0001-0001-0001-000000000003')
    INSERT INTO dbo.Products
        (ProductKey, ProductName, ProductNameAr, ProductSlag, ProductSlagAr,
         ProductDescription, ProductDescriptionAr, DeliveryInfo, DeliveryInfoArabic,
         AttributeCode, AttributeName, AttributeNameArabic, Barcode,
         ProductPrice, BrandId, FeatureCategoryId, StockQuantity,
         IsScheduled, ProductType, AvgRatings, Status, CreateDate, CreateBy)
    VALUES ('CC010003-0001-0001-0001-000000000003',
        'Greek Yogurt 500g', N'زبادي يوناني 500 جرام',
        'greek-yogurt-500g', N'زبادي-يوناني-500-جرام',
        'Thick and creamy Greek-style yogurt, 500g tub.',
        N'زبادي يوناني كثيف وكريمي، عبوة 500 جرام.',
        'Delivered within 1-2 business days', N'يتم التوصيل خلال 1-2 يوم عمل',
        '500G', '500 Grams', N'500 جرام', 'YOG-GRK-500G',
        4.75, @BrandIdGreenGrocer, @GroceryFCId, 250,
        0, 1, 0, 'Active', GETDATE(), 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE ProductKey = 'CC010004-0001-0001-0001-000000000004')
    INSERT INTO dbo.Products
        (ProductKey, ProductName, ProductNameAr, ProductSlag, ProductSlagAr,
         ProductDescription, ProductDescriptionAr, DeliveryInfo, DeliveryInfoArabic,
         AttributeCode, AttributeName, AttributeNameArabic, Barcode,
         ProductPrice, BrandId, FeatureCategoryId, StockQuantity,
         IsScheduled, ProductType, AvgRatings, Status, CreateDate, CreateBy)
    VALUES ('CC010004-0001-0001-0001-000000000004',
        'Cheddar Cheese 200g', N'جبنة شيدر 200 جرام',
        'cheddar-cheese-200g', N'جبنة-شيدر-200-جرام',
        'Mature cheddar cheese block, 200g.',
        N'قطعة جبنة شيدر ناضجة، 200 جرام.',
        'Delivered within 1-2 business days', N'يتم التوصيل خلال 1-2 يوم عمل',
        '200G', '200 Grams', N'200 جرام', 'CHE-CHD-200G',
        8.25, @BrandIdGreenGrocer, @GroceryFCId, 180,
        0, 1, 0, 'Active', GETDATE(), 1);

PRINT 'GreenGrocer products inserted.';

-- ============================================================
-- STEP 5: INSERT PRODUCTS – FreshFarm (Grains & Pantry)
-- ============================================================

IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE ProductKey = 'CC020001-0002-0002-0002-000000000001')
    INSERT INTO dbo.Products
        (ProductKey, ProductName, ProductNameAr, ProductSlag, ProductSlagAr,
         ProductDescription, ProductDescriptionAr, DeliveryInfo, DeliveryInfoArabic,
         AttributeCode, AttributeName, AttributeNameArabic, Barcode,
         ProductPrice, BrandId, FeatureCategoryId, StockQuantity,
         IsScheduled, ProductType, AvgRatings, Status, CreateDate, CreateBy)
    VALUES ('CC020001-0002-0002-0002-000000000001',
        'Basmati Rice 5Kg', N'أرز بسمتي 5 كيلو',
        'basmati-rice-5kg', N'أرز-بسمتي-5-كيلو',
        'Premium long-grain Basmati rice, 5 Kg bag.',
        N'أرز بسمتي فاخر طويل الحبة، كيس 5 كيلو.',
        'Delivered within 1-3 business days', N'يتم التوصيل خلال 1-3 أيام عمل',
        '5KG', '5 Kg', N'5 كيلو', 'RCE-BSM-5KG',
        18.00, @BrandIdFreshFarm, @GroceryFCId, 200,
        0, 1, 0, 'Active', GETDATE(), 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE ProductKey = 'CC020002-0002-0002-0002-000000000002')
    INSERT INTO dbo.Products
        (ProductKey, ProductName, ProductNameAr, ProductSlag, ProductSlagAr,
         ProductDescription, ProductDescriptionAr, DeliveryInfo, DeliveryInfoArabic,
         AttributeCode, AttributeName, AttributeNameArabic, Barcode,
         ProductPrice, BrandId, FeatureCategoryId, StockQuantity,
         IsScheduled, ProductType, AvgRatings, Status, CreateDate, CreateBy)
    VALUES ('CC020002-0002-0002-0002-000000000002',
        'Whole Wheat Bread 700g', N'خبز قمح كامل 700 جرام',
        'whole-wheat-bread-700g', N'خبز-قمح-كامل-700-جرام',
        'Freshly baked whole wheat loaf, 700g.',
        N'رغيف خبز قمح كامل طازج، 700 جرام.',
        'Delivered within 1-2 business days', N'يتم التوصيل خلال 1-2 يوم عمل',
        '700G', '700 Grams', N'700 جرام', 'BRD-WWT-700G',
        5.50, @BrandIdFreshFarm, @GroceryFCId, 150,
        0, 1, 0, 'Active', GETDATE(), 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE ProductKey = 'CC020003-0002-0002-0002-000000000003')
    INSERT INTO dbo.Products
        (ProductKey, ProductName, ProductNameAr, ProductSlag, ProductSlagAr,
         ProductDescription, ProductDescriptionAr, DeliveryInfo, DeliveryInfoArabic,
         AttributeCode, AttributeName, AttributeNameArabic, Barcode,
         ProductPrice, BrandId, FeatureCategoryId, StockQuantity,
         IsScheduled, ProductType, AvgRatings, Status, CreateDate, CreateBy)
    VALUES ('CC020003-0002-0002-0002-000000000003',
        'Extra Virgin Olive Oil 500ml', N'زيت زيتون بكر ممتاز 500 مل',
        'extra-virgin-olive-oil-500ml', N'زيت-زيتون-بكر-ممتاز-500-مل',
        'Cold-pressed extra virgin olive oil, 500 ml glass bottle.',
        N'زيت زيتون بكر ممتاز معصور على البارد، زجاجة 500 مل.',
        'Delivered within 1-3 business days', N'يتم التوصيل خلال 1-3 أيام عمل',
        '500ML', '500 ml', N'500 مل', 'OIL-OLV-500ML',
        22.50, @BrandIdFreshFarm, @GroceryFCId, 120,
        0, 1, 0, 'Active', GETDATE(), 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE ProductKey = 'CC020004-0002-0002-0002-000000000004')
    INSERT INTO dbo.Products
        (ProductKey, ProductName, ProductNameAr, ProductSlag, ProductSlagAr,
         ProductDescription, ProductDescriptionAr, DeliveryInfo, DeliveryInfoArabic,
         AttributeCode, AttributeName, AttributeNameArabic, Barcode,
         ProductPrice, BrandId, FeatureCategoryId, StockQuantity,
         IsScheduled, ProductType, AvgRatings, Status, CreateDate, CreateBy)
    VALUES ('CC020004-0002-0002-0002-000000000004',
        'Natural Honey 350g', N'عسل طبيعي 350 جرام',
        'natural-honey-350g', N'عسل-طبيعي-350-جرام',
        'Pure natural honey, 350g jar.',
        N'عسل طبيعي خالص، برطمان 350 جرام.',
        'Delivered within 1-3 business days', N'يتم التوصيل خلال 1-3 أيام عمل',
        '350G', '350 Grams', N'350 جرام', 'HNY-NAT-350G',
        15.00, @BrandIdFreshFarm, @GroceryFCId, 100,
        0, 1, 0, 'Active', GETDATE(), 1);

PRINT 'FreshFarm products inserted.';

-- ============================================================
-- STEP 6: INSERT PRODUCTS – OrganicWorld (Fresh Produce)
-- ============================================================

IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE ProductKey = 'CC030001-0003-0003-0003-000000000001')
    INSERT INTO dbo.Products
        (ProductKey, ProductName, ProductNameAr, ProductSlag, ProductSlagAr,
         ProductDescription, ProductDescriptionAr, DeliveryInfo, DeliveryInfoArabic,
         AttributeCode, AttributeName, AttributeNameArabic, Barcode,
         ProductPrice, BrandId, FeatureCategoryId, StockQuantity,
         IsScheduled, ProductType, AvgRatings, Status, CreateDate, CreateBy)
    VALUES ('CC030001-0003-0003-0003-000000000001',
        'Organic Apples 1Kg', N'تفاح عضوي 1 كيلو',
        'organic-apples-1kg', N'تفاح-عضوي-1-كيلو',
        'Certified organic Fuji apples, 1 Kg bag.',
        N'تفاح فوجي عضوي معتمد، كيس 1 كيلو.',
        'Delivered within 1-2 business days', N'يتم التوصيل خلال 1-2 يوم عمل',
        '1KG', '1 Kg', N'1 كيلو', 'APL-ORG-1KG',
        9.00, @BrandIdOrganicWorld, @GroceryFCId, 350,
        0, 1, 0, 'Active', GETDATE(), 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE ProductKey = 'CC030002-0003-0003-0003-000000000002')
    INSERT INTO dbo.Products
        (ProductKey, ProductName, ProductNameAr, ProductSlag, ProductSlagAr,
         ProductDescription, ProductDescriptionAr, DeliveryInfo, DeliveryInfoArabic,
         AttributeCode, AttributeName, AttributeNameArabic, Barcode,
         ProductPrice, BrandId, FeatureCategoryId, StockQuantity,
         IsScheduled, ProductType, AvgRatings, Status, CreateDate, CreateBy)
    VALUES ('CC030002-0003-0003-0003-000000000002',
        'Organic Bananas 500g', N'موز عضوي 500 جرام',
        'organic-bananas-500g', N'موز-عضوي-500-جرام',
        'Sweet organic bananas, approximately 500g bunch.',
        N'موز عضوي حلو، حزمة بوزن تقريبي 500 جرام.',
        'Delivered within 1-2 business days', N'يتم التوصيل خلال 1-2 يوم عمل',
        '500G', '500 Grams', N'500 جرام', 'BAN-ORG-500G',
        4.25, @BrandIdOrganicWorld, @GroceryFCId, 400,
        0, 1, 0, 'Active', GETDATE(), 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE ProductKey = 'CC030003-0003-0003-0003-000000000003')
    INSERT INTO dbo.Products
        (ProductKey, ProductName, ProductNameAr, ProductSlag, ProductSlagAr,
         ProductDescription, ProductDescriptionAr, DeliveryInfo, DeliveryInfoArabic,
         AttributeCode, AttributeName, AttributeNameArabic, Barcode,
         ProductPrice, BrandId, FeatureCategoryId, StockQuantity,
         IsScheduled, ProductType, AvgRatings, Status, CreateDate, CreateBy)
    VALUES ('CC030003-0003-0003-0003-000000000003',
        'Fresh Tomatoes 1Kg', N'طماطم طازجة 1 كيلو',
        'fresh-tomatoes-1kg', N'طماطم-طازجة-1-كيلو',
        'Vine-ripened fresh tomatoes, 1 Kg.',
        N'طماطم طازجة محصودة من الكرمة، 1 كيلو.',
        'Delivered within 1-2 business days', N'يتم التوصيل خلال 1-2 يوم عمل',
        '1KG', '1 Kg', N'1 كيلو', 'TOM-FRS-1KG',
        6.00, @BrandIdOrganicWorld, @GroceryFCId, 300,
        0, 1, 0, 'Active', GETDATE(), 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE ProductKey = 'CC030004-0003-0003-0003-000000000004')
    INSERT INTO dbo.Products
        (ProductKey, ProductName, ProductNameAr, ProductSlag, ProductSlagAr,
         ProductDescription, ProductDescriptionAr, DeliveryInfo, DeliveryInfoArabic,
         AttributeCode, AttributeName, AttributeNameArabic, Barcode,
         ProductPrice, BrandId, FeatureCategoryId, StockQuantity,
         IsScheduled, ProductType, AvgRatings, Status, CreateDate, CreateBy)
    VALUES ('CC030004-0003-0003-0003-000000000004',
        'Baby Spinach 200g', N'سبانخ صغير 200 جرام',
        'baby-spinach-200g', N'سبانخ-صغير-200-جرام',
        'Tender organic baby spinach leaves, 200g bag.',
        N'أوراق سبانخ صغيرة عضوية طرية، كيس 200 جرام.',
        'Delivered within 1-2 business days', N'يتم التوصيل خلال 1-2 يوم عمل',
        '200G', '200 Grams', N'200 جرام', 'SPN-BBY-200G',
        7.50, @BrandIdOrganicWorld, @GroceryFCId, 220,
        0, 1, 0, 'Active', GETDATE(), 1);

PRINT 'OrganicWorld products inserted.';

-- ============================================================
-- STEP 7: PRODUCT → CATEGORY LINKS
-- ============================================================

DECLARE @ProdId INT;

-- GreenGrocer → Dairy and Eggs
SELECT @ProdId = ProductId FROM dbo.Products WHERE ProductKey = 'CC010001-0001-0001-0001-000000000001';
IF NOT EXISTS (SELECT 1 FROM dbo.ProductCategories WHERE ProductId = @ProdId AND CategoryId = @CatIdDairy)
    INSERT INTO dbo.ProductCategories (ProductId, CategoryId, Status) VALUES (@ProdId, @CatIdDairy, 'Active');

SELECT @ProdId = ProductId FROM dbo.Products WHERE ProductKey = 'CC010002-0001-0001-0001-000000000002';
IF NOT EXISTS (SELECT 1 FROM dbo.ProductCategories WHERE ProductId = @ProdId AND CategoryId = @CatIdDairy)
    INSERT INTO dbo.ProductCategories (ProductId, CategoryId, Status) VALUES (@ProdId, @CatIdDairy, 'Active');

SELECT @ProdId = ProductId FROM dbo.Products WHERE ProductKey = 'CC010003-0001-0001-0001-000000000003';
IF NOT EXISTS (SELECT 1 FROM dbo.ProductCategories WHERE ProductId = @ProdId AND CategoryId = @CatIdDairy)
    INSERT INTO dbo.ProductCategories (ProductId, CategoryId, Status) VALUES (@ProdId, @CatIdDairy, 'Active');

SELECT @ProdId = ProductId FROM dbo.Products WHERE ProductKey = 'CC010004-0001-0001-0001-000000000004';
IF NOT EXISTS (SELECT 1 FROM dbo.ProductCategories WHERE ProductId = @ProdId AND CategoryId = @CatIdDairy)
    INSERT INTO dbo.ProductCategories (ProductId, CategoryId, Status) VALUES (@ProdId, @CatIdDairy, 'Active');

PRINT 'GreenGrocer linked to Dairy and Eggs.';

-- FreshFarm → Grains and Pantry
SELECT @ProdId = ProductId FROM dbo.Products WHERE ProductKey = 'CC020001-0002-0002-0002-000000000001';
IF NOT EXISTS (SELECT 1 FROM dbo.ProductCategories WHERE ProductId = @ProdId AND CategoryId = @CatIdGrains)
    INSERT INTO dbo.ProductCategories (ProductId, CategoryId, Status) VALUES (@ProdId, @CatIdGrains, 'Active');

SELECT @ProdId = ProductId FROM dbo.Products WHERE ProductKey = 'CC020002-0002-0002-0002-000000000002';
IF NOT EXISTS (SELECT 1 FROM dbo.ProductCategories WHERE ProductId = @ProdId AND CategoryId = @CatIdGrains)
    INSERT INTO dbo.ProductCategories (ProductId, CategoryId, Status) VALUES (@ProdId, @CatIdGrains, 'Active');

SELECT @ProdId = ProductId FROM dbo.Products WHERE ProductKey = 'CC020003-0002-0002-0002-000000000003';
IF NOT EXISTS (SELECT 1 FROM dbo.ProductCategories WHERE ProductId = @ProdId AND CategoryId = @CatIdGrains)
    INSERT INTO dbo.ProductCategories (ProductId, CategoryId, Status) VALUES (@ProdId, @CatIdGrains, 'Active');

SELECT @ProdId = ProductId FROM dbo.Products WHERE ProductKey = 'CC020004-0002-0002-0002-000000000004';
IF NOT EXISTS (SELECT 1 FROM dbo.ProductCategories WHERE ProductId = @ProdId AND CategoryId = @CatIdGrains)
    INSERT INTO dbo.ProductCategories (ProductId, CategoryId, Status) VALUES (@ProdId, @CatIdGrains, 'Active');

PRINT 'FreshFarm linked to Grains and Pantry.';

-- OrganicWorld → Fresh Produce
SELECT @ProdId = ProductId FROM dbo.Products WHERE ProductKey = 'CC030001-0003-0003-0003-000000000001';
IF NOT EXISTS (SELECT 1 FROM dbo.ProductCategories WHERE ProductId = @ProdId AND CategoryId = @CatIdProduce)
    INSERT INTO dbo.ProductCategories (ProductId, CategoryId, Status) VALUES (@ProdId, @CatIdProduce, 'Active');

SELECT @ProdId = ProductId FROM dbo.Products WHERE ProductKey = 'CC030002-0003-0003-0003-000000000002';
IF NOT EXISTS (SELECT 1 FROM dbo.ProductCategories WHERE ProductId = @ProdId AND CategoryId = @CatIdProduce)
    INSERT INTO dbo.ProductCategories (ProductId, CategoryId, Status) VALUES (@ProdId, @CatIdProduce, 'Active');

SELECT @ProdId = ProductId FROM dbo.Products WHERE ProductKey = 'CC030003-0003-0003-0003-000000000003';
IF NOT EXISTS (SELECT 1 FROM dbo.ProductCategories WHERE ProductId = @ProdId AND CategoryId = @CatIdProduce)
    INSERT INTO dbo.ProductCategories (ProductId, CategoryId, Status) VALUES (@ProdId, @CatIdProduce, 'Active');

SELECT @ProdId = ProductId FROM dbo.Products WHERE ProductKey = 'CC030004-0003-0003-0003-000000000004';
IF NOT EXISTS (SELECT 1 FROM dbo.ProductCategories WHERE ProductId = @ProdId AND CategoryId = @CatIdProduce)
    INSERT INTO dbo.ProductCategories (ProductId, CategoryId, Status) VALUES (@ProdId, @CatIdProduce, 'Active');

PRINT 'OrganicWorld linked to Fresh Produce.';

-- ============================================================
-- VERIFICATION
-- ============================================================
PRINT '=== FINAL VERIFICATION ===';

SELECT b.BrandId, b.Title AS SellerName, b.Status,
       COUNT(p.ProductId) AS ProductCount
FROM dbo.Brands b
LEFT JOIN dbo.Products p ON p.BrandId = b.BrandId
    AND p.FeatureCategoryId = @GroceryFCId
    AND p.DeleteDate IS NULL
WHERE b.BrandKey IN (
    'AA000001-0001-0001-0001-000000000001',
    'AA000002-0002-0002-0002-000000000002',
    'AA000003-0003-0003-0003-000000000003'
)
GROUP BY b.BrandId, b.Title, b.Status;

SELECT p.ProductId, p.ProductName, p.ProductPrice,
       b.Title AS SellerName, c.CategoryName
FROM dbo.Products p
INNER JOIN dbo.Brands b ON b.BrandId = p.BrandId
LEFT  JOIN dbo.ProductCategories pc ON pc.ProductId = p.ProductId
LEFT  JOIN dbo.Categories c ON c.CategoryId = pc.CategoryId
WHERE b.BrandKey IN (
    'AA000001-0001-0001-0001-000000000001',
    'AA000002-0002-0002-0002-000000000002',
    'AA000003-0003-0003-0003-000000000003'
)
ORDER BY b.Title, p.ProductName;

PRINT 'Done.';
