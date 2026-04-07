-- ============================================================
-- reset_orders_grocery.sql
-- 1) Deletes ALL orders for the Grocery feature category
-- 2) Inserts ONE new multi-seller order (3 sellers, 6 products)
-- ============================================================
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET NOCOUNT ON;

DECLARE @GroceryFCId INT = 2;

-- ============================================================
-- STEP 1: DELETE ALL GROCERY ORDERS
-- ============================================================

-- 1a. Delete status logs for grocery orders
DELETE omsl
FROM dbo.OrderMasterStatusLogs omsl
INNER JOIN dbo.OrderRequestProducts o
    ON o.OrderRequestProductId = omsl.OrderId
WHERE o.FeatureCategoryId = @GroceryFCId;

PRINT 'OrderMasterStatusLogs deleted: ' + CAST(@@ROWCOUNT AS VARCHAR);

-- 1b. Delete order details
DELETE od
FROM dbo.OrderRequestProductDetails od
INNER JOIN dbo.OrderRequestProducts o
    ON o.OrderRequestProductId = od.OrderRequestProductId
WHERE o.FeatureCategoryId = @GroceryFCId;

PRINT 'OrderRequestProductDetails deleted: ' + CAST(@@ROWCOUNT AS VARCHAR);

-- 1c. Delete the orders themselves
DELETE FROM dbo.OrderRequestProducts
WHERE FeatureCategoryId = @GroceryFCId;

PRINT 'OrderRequestProducts deleted: ' + CAST(@@ROWCOUNT AS VARCHAR);

-- ============================================================
-- STEP 2: INSERT ONE MULTI-SELLER ORDER
-- ============================================================

-- Get product IDs for our 3 sellers
DECLARE @ProdMilk   INT, @ProdEggs   INT,   -- GreenGrocer
        @ProdRice   INT, @ProdHoney  INT,   -- FreshFarm
        @ProdApples INT, @ProdSpinach INT;  -- OrganicWorld

SELECT @ProdMilk    = ProductId FROM dbo.Products WHERE ProductKey = 'CC010001-0001-0001-0001-000000000001';
SELECT @ProdEggs    = ProductId FROM dbo.Products WHERE ProductKey = 'CC010002-0001-0001-0001-000000000002';
SELECT @ProdRice    = ProductId FROM dbo.Products WHERE ProductKey = 'CC020001-0002-0002-0002-000000000001';
SELECT @ProdHoney   = ProductId FROM dbo.Products WHERE ProductKey = 'CC020004-0002-0002-0002-000000000004';
SELECT @ProdApples  = ProductId FROM dbo.Products WHERE ProductKey = 'CC030001-0003-0003-0003-000000000001';
SELECT @ProdSpinach = ProductId FROM dbo.Products WHERE ProductKey = 'CC030004-0003-0003-0003-000000000004';

PRINT 'ProdMilk='    + ISNULL(CAST(@ProdMilk    AS VARCHAR), 'NULL');
PRINT 'ProdEggs='    + ISNULL(CAST(@ProdEggs    AS VARCHAR), 'NULL');
PRINT 'ProdRice='    + ISNULL(CAST(@ProdRice    AS VARCHAR), 'NULL');
PRINT 'ProdHoney='   + ISNULL(CAST(@ProdHoney   AS VARCHAR), 'NULL');
PRINT 'ProdApples='  + ISNULL(CAST(@ProdApples  AS VARCHAR), 'NULL');
PRINT 'ProdSpinach=' + ISNULL(CAST(@ProdSpinach AS VARCHAR), 'NULL');

-- Prices: Milk=3.50, Eggs=6.50, Rice=18.00, Honey=15.00, Apples=9.00, Spinach=7.50
-- Quantities: 2, 1, 1, 2, 3, 1
-- Subtotals:  7.00+6.50+18.00+30.00+27.00+7.50 = 96.00
DECLARE @TotalPrice FLOAT = (3.50*2) + (6.50*1) + (18.00*1) + (15.00*2) + (9.00*3) + (7.50*1);
-- = 7.00 + 6.50 + 18.00 + 30.00 + 27.00 + 7.50 = 96.00
DECLARE @DeliveryCharge FLOAT = 5.00;

INSERT INTO dbo.OrderRequestProducts
    (ReadableOrderId, MemberId, MemberAddressId,
     OrderDateTime, DeliveryDateTime,
     Comments, DeliveryCharge, TotalPrice,
     PaymentMethodId, Status, CreateBy, CreateDate,
     OrderStatusId, FeatureCategoryId,
     PaymentStatus, IsSound, IsAdmin,
     ServiceCharge, Tip, ProductType, IsCanceled)
VALUES
    ('GRC-001', 1, NULL,
     GETDATE(), DATEADD(DAY, 2, GETDATE()),
     N'Multi-seller test order – GreenGrocer + FreshFarm + OrganicWorld',
     @DeliveryCharge, @TotalPrice + @DeliveryCharge,
     1, 'Active', 1, GETDATE(),
     1, @GroceryFCId,
     'Pending', 0, 1,
     0, 0, 1, 0);

DECLARE @NewOrderId INT = SCOPE_IDENTITY();
PRINT 'New OrderRequestProductId = ' + CAST(@NewOrderId AS VARCHAR);

-- Insert order details – one row per product line
INSERT INTO dbo.OrderRequestProductDetails
    (OrderRequestProductId, ProductId, Quantity, GrossPrice,
     Status, CreateBy, CreateDate,
     IsMembershipOrder, DiscountAmount, MembershipId)
VALUES
    -- GreenGrocer: Fresh Whole Milk 1L × 2
    (@NewOrderId, @ProdMilk,    2, 3.50, 'Active', 1, GETDATE(), 0, 0, 0),
    -- GreenGrocer: Free Range Eggs 12pc × 1
    (@NewOrderId, @ProdEggs,    1, 6.50, 'Active', 1, GETDATE(), 0, 0, 0),
    -- FreshFarm: Basmati Rice 5Kg × 1
    (@NewOrderId, @ProdRice,    1, 18.00,'Active', 1, GETDATE(), 0, 0, 0),
    -- FreshFarm: Natural Honey 350g × 2
    (@NewOrderId, @ProdHoney,   2, 15.00,'Active', 1, GETDATE(), 0, 0, 0),
    -- OrganicWorld: Organic Apples 1Kg × 3
    (@NewOrderId, @ProdApples,  3, 9.00, 'Active', 1, GETDATE(), 0, 0, 0),
    -- OrganicWorld: Baby Spinach 200g × 1
    (@NewOrderId, @ProdSpinach, 1, 7.50, 'Active', 1, GETDATE(), 0, 0, 0);

PRINT 'Order details inserted: ' + CAST(@@ROWCOUNT AS VARCHAR);

-- ============================================================
-- VERIFICATION
-- ============================================================
PRINT '=== ORDER SUMMARY ===';
SELECT o.OrderRequestProductId, o.ReadableOrderId,
       o.TotalPrice, o.DeliveryCharge, o.Status
FROM dbo.OrderRequestProducts o
WHERE o.OrderRequestProductId = @NewOrderId;

PRINT '=== ORDER ITEMS BY SELLER ===';
SELECT b.Title AS SellerName,
       p.ProductName,
       d.Quantity,
       d.GrossPrice,
       d.GrossPrice * d.Quantity AS LineTotal
FROM dbo.OrderRequestProductDetails d
INNER JOIN dbo.Products p ON p.ProductId = d.ProductId
INNER JOIN dbo.Brands   b ON b.BrandId   = p.BrandId
WHERE d.OrderRequestProductId = @NewOrderId
ORDER BY b.Title, p.ProductName;

PRINT 'Done.';
