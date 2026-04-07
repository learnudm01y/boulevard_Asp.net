-- ============================================================
-- Delete all Grocery product + category data
-- Run BEFORE re-importing insert_grocery.xlsx
--
-- Safe: wrapped in a transaction; rolls back on any error.
-- Grocery FeatureCategoryKey = 3b317e3f-cb2f-4fdd-b9c8-3f2186695771
-- ============================================================

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

BEGIN TRANSACTION;

BEGIN TRY

    -- Resolve the integer ID first
    DECLARE @fcId INT = (
        SELECT FeatureCategoryId
        FROM dbo.FeatureCategories
        WHERE FeatureCategoryKey = '3b317e3f-cb2f-4fdd-b9c8-3f2186695771'
    );

    IF @fcId IS NULL
    BEGIN
        RAISERROR('Grocery FeatureCategory not found. Run seed_grocery.sql first.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    PRINT CONCAT('Deleting grocery data for FeatureCategoryId = ', @fcId, ' ...');

    -- 1. Favourite products
    DELETE fp
    FROM dbo.FavouriteProducts fp
    INNER JOIN dbo.Products p ON fp.ProductId = p.ProductId
    WHERE p.FeatureCategoryId = @fcId;

    -- 2. Cart lines
    DELETE c
    FROM dbo.Carts c
    INNER JOIN dbo.Products p ON c.ProductId = p.ProductId
    WHERE p.FeatureCategoryId = @fcId;

    -- 3. Order request product details (references both Products and ProductPrices)
    DELETE ord
    FROM dbo.OrderRequestProductDetails ord
    INNER JOIN dbo.Products p ON ord.ProductId = p.ProductId
    WHERE p.FeatureCategoryId = @fcId;

    -- 4. Upsell / Cross-sell links (tables may not exist in all DB versions)
    IF OBJECT_ID('dbo.UpsellProducts', 'U') IS NOT NULL
        DELETE FROM dbo.UpsellProducts WHERE ProductId IN (SELECT ProductId FROM dbo.Products WHERE FeatureCategoryId = @fcId);
    IF OBJECT_ID('dbo.CrosssellProductDetails', 'U') IS NOT NULL
        DELETE FROM dbo.CrosssellProductDetails WHERE ProductId IN (SELECT ProductId FROM dbo.Products WHERE FeatureCategoryId = @fcId);

    -- 5. Common product tag details
    IF OBJECT_ID('dbo.CommonProductTagDetails', 'U') IS NOT NULL
    BEGIN
        DELETE ctd
        FROM dbo.CommonProductTagDetails ctd
        INNER JOIN dbo.Products p ON ctd.ProductId = p.ProductId
        WHERE p.FeatureCategoryId = @fcId;
    END

    -- 6. Product offers
    IF OBJECT_ID('dbo.ProductOffers', 'U') IS NOT NULL
        DELETE po FROM dbo.ProductOffers po
        INNER JOIN dbo.Products p ON po.ProductId = p.ProductId
        WHERE p.FeatureCategoryId = @fcId;

    -- 7. Stock logs
    IF OBJECT_ID('dbo.StockLogs', 'U') IS NOT NULL
    BEGIN
        DELETE sl FROM dbo.StockLogs sl
        INNER JOIN dbo.Products p ON sl.ProductId = p.ProductId
        WHERE p.FeatureCategoryId = @fcId;
    END

    -- 8. Product prices
    DELETE pp
    FROM dbo.ProductPrices pp
    INNER JOIN dbo.Products p ON pp.ProductId = p.ProductId
    WHERE p.FeatureCategoryId = @fcId;

    -- 9. Product images
    DELETE pi
    FROM dbo.ProductImages pi
    INNER JOIN dbo.Products p ON pi.ProductId = p.ProductId
    WHERE p.FeatureCategoryId = @fcId;

    -- 10. Product–category links
    DELETE pc
    FROM dbo.ProductCategories pc
    INNER JOIN dbo.Products p ON pc.ProductId = p.ProductId
    WHERE p.FeatureCategoryId = @fcId;

    -- 11. Products themselves
    DELETE FROM dbo.Products WHERE FeatureCategoryId = @fcId;

    -- 12. Brand offers (before deleting Brands)
    IF OBJECT_ID('dbo.BrandOffers', 'U') IS NOT NULL
        DELETE bo FROM dbo.BrandOffers bo
        INNER JOIN dbo.Brands b ON bo.BrandId = b.BrandId
        WHERE b.FeatureCategoryId = @fcId
          AND b.BrandId NOT IN (SELECT DISTINCT BrandId FROM dbo.Products WHERE BrandId IS NOT NULL);

    -- 13. Brands linked to Grocery AND no longer referenced by any product in any category
    DELETE FROM dbo.Brands
    WHERE FeatureCategoryId = @fcId
      AND BrandId NOT IN (SELECT DISTINCT BrandId FROM dbo.Products WHERE BrandId IS NOT NULL);

    -- 14. Category offers (before deleting Categories)
    IF OBJECT_ID('dbo.CategoryOffers', 'U') IS NOT NULL
        DELETE co FROM dbo.CategoryOffers co
        INNER JOIN dbo.Categories c ON co.CategoryId = c.CategoryId
        WHERE c.FeatureCategoryId = @fcId;

    -- 15. Categories (all levels – walk from deepest to shallowest)
    --     Level 3 first, then 2, then 1 (ParentId = 0)
    DELETE c3
    FROM dbo.Categories c3
    WHERE c3.FeatureCategoryId = @fcId
      AND c3.ParentId IN (
          SELECT CategoryId FROM dbo.Categories
          WHERE FeatureCategoryId = @fcId AND ParentId != 0
      );

    DELETE FROM dbo.Categories
    WHERE FeatureCategoryId = @fcId AND ParentId != 0;

    DELETE FROM dbo.Categories
    WHERE FeatureCategoryId = @fcId AND ParentId = 0;

    -- 16. Clear the temp staging table
    DELETE FROM dbo.TempProducts WHERE FeatureCategoryId = @fcId;

    PRINT 'Grocery data deleted successfully.';

    COMMIT TRANSACTION;

END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT CONCAT('ERROR: ', ERROR_MESSAGE());
    THROW;
END CATCH;
