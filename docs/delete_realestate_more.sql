SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET ANSI_WARNINGS ON;
DECLARE @guid UNIQUEIDENTIFIER = CONVERT(UNIQUEIDENTIFIER, 'DD501B2D-FE22-4C31-B340-1B4237FAB5CC');
DECLARE @fcId INT = (SELECT FeatureCategoryId FROM FeatureCategories WHERE FeatureCategoryKey = @guid);
PRINT 'FeatureCategoryId=' + ISNULL(CONVERT(VARCHAR(10),@fcId),'NULL');

DECLARE @tables TABLE(Name NVARCHAR(200));
INSERT INTO @tables(Name) VALUES
('OfferInformations'),('OfferBanner'),('BrandOffers'),('CategoryOffers'),('ProductOffers'),('ServiceOffers'),
('OfferDiscounts'),('Services'),('ServiceTypes'),('ServiceCategories'),('ServiceAmenities'),('Products'),
('ProductCategories'),('FaqServices'),('webHtmls'),('FavouriteServices'),('FavouriteProducts'),('ServiceImages'),
('ServiceTypeFiles'),('ServiceTypeAmenities'),('ServiceTypeFiles');

DECLARE @t NVARCHAR(200);
DECLARE cur CURSOR FOR SELECT Name FROM @tables;
OPEN cur; FETCH NEXT FROM cur INTO @t;
WHILE @@FETCH_STATUS=0
BEGIN
    IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @t)
    BEGIN
        IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @t AND COLUMN_NAME = 'FeatureCategoryKey')
        BEGIN
            DECLARE @s NVARCHAR(MAX) = N'DELETE FROM [' + @t + N'] WHERE FeatureCategoryKey = ''' + CONVERT(NVARCHAR(36), @guid) + N'''; SELECT ''' + @t + N' ''' + ' AS Tbl, CAST(@@ROWCOUNT AS NVARCHAR(20)) AS DeletedCount;';
            EXEC sp_executesql @s;
        END
    END
    FETCH NEXT FROM cur INTO @t;
END
CLOSE cur; DEALLOCATE cur;

-- Clean up dependent rows that reference ServiceTypes/Services by Id
IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ServiceTypeAmenities')
BEGIN
    DELETE FROM ServiceTypeAmenities WHERE ServiceTypeId IN (
        SELECT st.ServiceTypeId FROM ServiceTypes st
        JOIN Services s ON st.ServiceId = s.ServiceId
        WHERE s.FeatureCategoryId = @fcId
    );
    PRINT 'Deleted dependent ServiceTypeAmenities rows.';
END
IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ServiceTypeFiles')
BEGIN
    DELETE FROM ServiceTypeFiles WHERE ServiceTypeId IN (
        SELECT st.ServiceTypeId FROM ServiceTypes st
        JOIN Services s ON st.ServiceId = s.ServiceId
        WHERE s.FeatureCategoryId = @fcId
    );
    PRINT 'Deleted dependent ServiceTypeFiles rows.';
END
IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ServiceImages')
BEGIN
    DELETE FROM ServiceImages WHERE ServiceId IN (SELECT ServiceId FROM Services WHERE FeatureCategoryId = @fcId);
    PRINT 'Deleted dependent ServiceImages rows.';
END
IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ServiceOffers')
BEGIN
    DELETE FROM ServiceOffers WHERE ServiceId IN (SELECT ServiceId FROM Services WHERE FeatureCategoryId = @fcId);
    PRINT 'Deleted dependent ServiceOffers rows.';
END

-- Delete rows in any table that reference ServiceTypeId for ServiceTypes belonging to this FeatureCategory
DECLARE @stTables TABLE(Name NVARCHAR(200));
INSERT INTO @stTables(Name)
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'ServiceTypeId' AND TABLE_NAME NOT IN ('ServiceTypeAmenities','ServiceTypeFiles');

DECLARE @stn NVARCHAR(200);
DECLARE cur_st CURSOR FOR SELECT Name FROM @stTables;
OPEN cur_st; FETCH NEXT FROM cur_st INTO @stn;
WHILE @@FETCH_STATUS = 0
BEGIN
    IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @stn AND COLUMN_NAME = 'ServiceTypeId')
    BEGIN
        DECLARE @sqlst NVARCHAR(MAX) = N'DELETE FROM [' + @stn + N'] WHERE ServiceTypeId IN (SELECT st.ServiceTypeId FROM ServiceTypes st JOIN Services s ON st.ServiceId = s.ServiceId WHERE s.FeatureCategoryId = @fcIdParam);';
        BEGIN TRY
            EXEC sp_executesql @sqlst, N'@fcIdParam INT', @fcIdParam = @fcId;
            PRINT 'Deleted rows from ' + @stn + ' by ServiceTypeId for FeatureCategoryId=' + CONVERT(VARCHAR(10),@fcId);
        END TRY
        BEGIN CATCH
            PRINT 'ERROR deleting ServiceTypeId refs from ' + @stn + ': ' + ERROR_MESSAGE();
        END CATCH
    END
    FETCH NEXT FROM cur_st INTO @stn;
END
CLOSE cur_st; DEALLOCATE cur_st;

-- Delete rows in any table that reference ServiceId for Services belonging to this FeatureCategory
DECLARE @sTables TABLE(Name NVARCHAR(200));
INSERT INTO @sTables(Name)
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'ServiceId' AND TABLE_NAME NOT IN ('ServiceImages','ServiceOffers','ServiceTypes','Services');

DECLARE @sn NVARCHAR(200);
DECLARE cur_s CURSOR FOR SELECT Name FROM @sTables;
OPEN cur_s; FETCH NEXT FROM cur_s INTO @sn;
WHILE @@FETCH_STATUS = 0
BEGIN
    IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @sn AND COLUMN_NAME = 'ServiceId')
    BEGIN
        DECLARE @sqls NVARCHAR(MAX) = N'DELETE FROM [' + @sn + N'] WHERE ServiceId IN (SELECT ServiceId FROM Services WHERE FeatureCategoryId = @fcIdParam);';
        BEGIN TRY
            EXEC sp_executesql @sqls, N'@fcIdParam INT', @fcIdParam = @fcId;
            PRINT 'Deleted rows from ' + @sn + ' by ServiceId for FeatureCategoryId=' + CONVERT(VARCHAR(10),@fcId);
        END TRY
        BEGIN CATCH
            PRINT 'ERROR deleting ServiceId refs from ' + @sn + ': ' + ERROR_MESSAGE();
        END CATCH
    END
    FETCH NEXT FROM cur_s INTO @sn;
END
CLOSE cur_s; DEALLOCATE cur_s;


-- Attempt deletes for all tables that have FeatureCategoryId column (safe TRY/CATCH)
DECLARE @delTables TABLE(Name NVARCHAR(200));
INSERT INTO @delTables(Name)
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'FeatureCategoryId' AND TABLE_NAME NOT IN ('FeatureCategories');

DECLARE @tn NVARCHAR(200);
DECLARE cur2 CURSOR FOR SELECT Name FROM @delTables;
OPEN cur2; FETCH NEXT FROM cur2 INTO @tn;
WHILE @@FETCH_STATUS = 0
BEGIN
    IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @tn AND COLUMN_NAME = 'FeatureCategoryId')
    BEGIN
        DECLARE @sql NVARCHAR(MAX) = N'DELETE FROM [' + @tn + N'] WHERE FeatureCategoryId = @fcIdParam; SELECT ''' + @tn + N''' AS Tbl, CAST(@@ROWCOUNT AS NVARCHAR(20)) AS DeletedCount;';
        BEGIN TRY
            EXEC sp_executesql @sql, N'@fcIdParam INT', @fcIdParam = @fcId;
        END TRY
        BEGIN CATCH
            PRINT 'ERROR deleting from ' + @tn + ': ' + ERROR_MESSAGE();
        END CATCH
    END
    FETCH NEXT FROM cur2 INTO @tn;
END
CLOSE cur2; DEALLOCATE cur2;

-- Delete order requests by numeric FeatureCategoryId if present
IF @fcId IS NOT NULL
BEGIN
    -- delete details first
    DELETE FROM OrderRequestServiceDetails WHERE OrderRequestServiceId IN (SELECT OrderRequestServiceId FROM OrderRequestServices WHERE FeatureCategoryId = @fcId);
    PRINT 'Deleted OrderRequestServiceDetails for FeatureCategoryId=' + CONVERT(VARCHAR(10),@fcId);
    DELETE FROM OrderRequestServices WHERE FeatureCategoryId = @fcId;
    PRINT 'Deleted OrderRequestService rows for FeatureCategoryId=' + CONVERT(VARCHAR(10),@fcId);
END

-- Finally remove the FeatureCategory record itself
DELETE FROM FeatureCategories WHERE FeatureCategoryKey = @guid;
PRINT 'Deleted FeatureCategories row for GUID if existed.';

PRINT 'Script complete.';
