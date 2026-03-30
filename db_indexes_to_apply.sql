-- ============================================================
-- Boulevard Database Performance Indexes
-- Run this script on the PRODUCTION database server.
-- All indexes use IF NOT EXISTS guards to be safe to re-run.
-- ============================================================

-- ============================================================
-- NOTE: The following 7 indexes were already applied via EF
-- migration (202607010500000_AddDashboardPerformanceIndexes).
-- They are listed here only for documentation purposes.
-- ============================================================
-- IX_OrderRequestProducts_OrderDateTime      ON dbo.OrderRequestProducts (OrderDateTime)
-- IX_OrderRequestProducts_DeliveryDateTime   ON dbo.OrderRequestProducts (DeliveryDateTime)
-- IX_OrderRequestProducts_CreateDate         ON dbo.OrderRequestProducts (CreateDate)
-- IX_OrderRequestProducts_FeatureCategoryId  ON dbo.OrderRequestProducts (FeatureCategoryId)
-- IX_OrderRequestServices_BookingDate        ON dbo.OrderRequestServices (BookingDate)
-- IX_OrderRequestServices_FeatureCategoryId  ON dbo.OrderRequestServices (FeatureCategoryId)
-- IX_Members_Status                          ON dbo.Members (Status)
-- ============================================================

USE [BoulevardDb];   -- <<<< Change to your actual database name if different
GO

-- ============================================================
-- dbo.OrderRequestProducts
-- ============================================================

-- Composite index for the main order list query:
-- WHERE Status = 'Active' AND OrderStatusId = @statusId
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_OrderRequestProducts_Status_OrderStatusId' AND object_id = OBJECT_ID('dbo.OrderRequestProducts'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_OrderRequestProducts_Status_OrderStatusId
    ON dbo.OrderRequestProducts (Status, OrderStatusId)
    INCLUDE (OrderRequestProductId, ReadableOrderId, MemberId, OrderDateTime, DeliveryDateTime, TotalPrice, FeatureCategoryId);
    PRINT 'Created: IX_OrderRequestProducts_Status_OrderStatusId';
END
ELSE
    PRINT 'Already exists: IX_OrderRequestProducts_Status_OrderStatusId';
GO

-- Index for MemberId (joins and lookups)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_OrderRequestProducts_MemberId' AND object_id = OBJECT_ID('dbo.OrderRequestProducts'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_OrderRequestProducts_MemberId
    ON dbo.OrderRequestProducts (MemberId);
    PRINT 'Created: IX_OrderRequestProducts_MemberId';
END
ELSE
    PRINT 'Already exists: IX_OrderRequestProducts_MemberId';
GO

-- Index for IsSound (used by TriggerAction to find unprocessed orders)
-- NOTE: QUOTED_IDENTIFIER ON required for filtered indexes
SET QUOTED_IDENTIFIER ON;
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_OrderRequestProducts_IsSound' AND object_id = OBJECT_ID('dbo.OrderRequestProducts'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_OrderRequestProducts_IsSound
    ON dbo.OrderRequestProducts (IsSound)
    WHERE IsSound = 0;    -- filtered index: only un-processed rows
    PRINT 'Created: IX_OrderRequestProducts_IsSound';
END
ELSE
    PRINT 'Already exists: IX_OrderRequestProducts_IsSound';
GO

-- Index for OrderStatusId alone (status-only queries & UpdateStatus)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_OrderRequestProducts_OrderStatusId' AND object_id = OBJECT_ID('dbo.OrderRequestProducts'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_OrderRequestProducts_OrderStatusId
    ON dbo.OrderRequestProducts (OrderStatusId);
    PRINT 'Created: IX_OrderRequestProducts_OrderStatusId';
END
ELSE
    PRINT 'Already exists: IX_OrderRequestProducts_OrderStatusId';
GO

-- ============================================================
-- dbo.OrderRequestServices
-- ============================================================

-- Index for MemberId
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_OrderRequestServices_MemberId' AND object_id = OBJECT_ID('dbo.OrderRequestServices'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_OrderRequestServices_MemberId
    ON dbo.OrderRequestServices (MemberId);
    PRINT 'Created: IX_OrderRequestServices_MemberId';
END
ELSE
    PRINT 'Already exists: IX_OrderRequestServices_MemberId';
GO

-- Index for IsSound (TriggerAction)
SET QUOTED_IDENTIFIER ON;
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_OrderRequestServices_IsSound' AND object_id = OBJECT_ID('dbo.OrderRequestServices'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_OrderRequestServices_IsSound
    ON dbo.OrderRequestServices (IsSound)
    WHERE IsSound = 0;
    PRINT 'Created: IX_OrderRequestServices_IsSound';
END
ELSE
    PRINT 'Already exists: IX_OrderRequestServices_IsSound';
GO

-- NOTE: OrderRequestServices uses BookingStatus (nvarchar(max)) instead of an
-- integer OrderStatusId FK, so no OrderStatusId index is needed for that table.
-- BookingStatus is nvarchar(max) and cannot be a key column in an index;
-- the main slow query (DISTINCT BookingStatus scan) has been removed from the
-- controller and replaced with a hardcoded list, so no index is needed here.
GO

-- ============================================================
-- dbo.Members
-- ============================================================

-- Index for PhoneNumber (member search & duplicate check in SaveMember)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Members_PhoneNumber' AND object_id = OBJECT_ID('dbo.Members'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Members_PhoneNumber
    ON dbo.Members (PhoneNumber);
    PRINT 'Created: IX_Members_PhoneNumber';
END
ELSE
    PRINT 'Already exists: IX_Members_PhoneNumber';
GO

-- Index for Email
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Members_Email' AND object_id = OBJECT_ID('dbo.Members'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Members_Email
    ON dbo.Members (Email);
    PRINT 'Created: IX_Members_Email';
END
ELSE
    PRINT 'Already exists: IX_Members_Email';
GO

-- Index for MemberKey (GUID lookups for API/mobile requests)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Members_MemberKey' AND object_id = OBJECT_ID('dbo.Members'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX IX_Members_MemberKey
    ON dbo.Members (MemberKey);
    PRINT 'Created: IX_Members_MemberKey';
END
ELSE
    PRINT 'Already exists: IX_Members_MemberKey';
GO

-- ============================================================
-- dbo.FeatureCategories
-- ============================================================

-- Index for FeatureCategoryKey (GUID lookup used in every category-filtered query)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_FeatureCategories_FeatureCategoryKey' AND object_id = OBJECT_ID('dbo.FeatureCategories'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX IX_FeatureCategories_FeatureCategoryKey
    ON dbo.FeatureCategories (FeatureCategoryKey);
    PRINT 'Created: IX_FeatureCategories_FeatureCategoryKey';
END
ELSE
    PRINT 'Already exists: IX_FeatureCategories_FeatureCategoryKey';
GO

-- ============================================================
-- dbo.OrderRequestProductDetails
-- ============================================================

-- Index for OrderRequestProductId (Details page: loads all line items for one order)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_OrderRequestProductDetails_OrderRequestProductId' AND object_id = OBJECT_ID('dbo.OrderRequestProductDetails'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_OrderRequestProductDetails_OrderRequestProductId
    ON dbo.OrderRequestProductDetails (OrderRequestProductId);
    PRINT 'Created: IX_OrderRequestProductDetails_OrderRequestProductId';
END
ELSE
    PRINT 'Already exists: IX_OrderRequestProductDetails_OrderRequestProductId';
GO

-- ============================================================
-- dbo.OrderRequestServiceDetails
-- ============================================================

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_OrderRequestServiceDetails_OrderRequestServiceId' AND object_id = OBJECT_ID('dbo.OrderRequestServiceDetails'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_OrderRequestServiceDetails_OrderRequestServiceId
    ON dbo.OrderRequestServiceDetails (OrderRequestServiceId);
    PRINT 'Created: IX_OrderRequestServiceDetails_OrderRequestServiceId';
END
ELSE
    PRINT 'Already exists: IX_OrderRequestServiceDetails_OrderRequestServiceId';
GO

-- ============================================================
-- dbo.Products
-- ============================================================

-- Composite index for product listing filtered by status and category
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Products_Status_FeatureCategoryId' AND object_id = OBJECT_ID('dbo.Products'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Products_Status_FeatureCategoryId
    ON dbo.Products (Status, FeatureCategoryId)
    INCLUDE (ProductId, ProductKey, ProductName, ProductPrice, StockQuantity);
    PRINT 'Created: IX_Products_Status_FeatureCategoryId';
END
ELSE
    PRINT 'Already exists: IX_Products_Status_FeatureCategoryId';
GO

-- Filtered index on ProductName for search/LIKE queries
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Products_ProductName_Active' AND object_id = OBJECT_ID('dbo.Products'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Products_ProductName_Active
    ON dbo.Products (ProductName)
    WHERE Status = 'Active';
    PRINT 'Created: IX_Products_ProductName_Active';
END
ELSE
    PRINT 'Already exists: IX_Products_ProductName_Active';
GO

-- ============================================================
-- dbo.Services
-- ============================================================

-- Composite index for service listing filtered by status and category
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Services_Status_FeatureCategoryId' AND object_id = OBJECT_ID('dbo.Services'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Services_Status_FeatureCategoryId
    ON dbo.Services (Status, FeatureCategoryId);
    PRINT 'Created: IX_Services_Status_FeatureCategoryId';
END
ELSE
    PRINT 'Already exists: IX_Services_Status_FeatureCategoryId';
GO

PRINT '============================================================';
PRINT 'All Boulevard performance indexes applied successfully.';
PRINT '============================================================';
