using Boulevard.Areas.Admin.Data;
using Boulevard.BaseRepository;
using Boulevard.Contexts;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Configuration;
using Boulevard.Models;
using Boulevard.Helper;
using System.Xml.Linq;
using Boulevard.Enum;
using Boulevard.Service.WebAPI;
using static Boulevard.Service.WebAPI.SendPushNotificationNewVersion;


namespace Boulevard.Service.Admin
{
    public class TempProductDataAccess
    {
        public IUnitOfWork uow;

        public TempProductDataAccess()
        {
            uow = new UnitOfWork();
        }

        public TempProductCountViewModel GetTempProductCount()
        {
            var db = new BoulevardDbContext();
            TempProductCountViewModel tempProductCount = new TempProductCountViewModel();

            int DoneCount = db.TempProducts.Count();
            int TotalDuplicate = (from temp in db.TempProducts
                                  where db.Products.Any(f => f.ProductName == temp.ProductName)
                                  select temp).Count();

            int TotalNew = (from temp in db.TempProducts
                            where !db.Products.Any(f => f.ProductName == temp.ProductName)
                            select temp).Count();

            tempProductCount.DoneCount = DoneCount;
            if (db.TempProducts.Count() > 0)
            {
                var db1 = new BoulevardDbContext();
                tempProductCount.TotalCount = db1.TempProducts.Count() > 0 ? db1.TempProducts.FirstOrDefault().ExcelCount : 0;
            }
            //tempProductCount.TotalCount = db.TempProducts.Count() > 0 ? db.TempProducts.FirstOrDefault().ExcelCount : 0;
            tempProductCount.TotalNew = TotalNew;
            tempProductCount.TotalDuplicate = TotalDuplicate;

            return tempProductCount;
        }

        /// <summary>
        /// Deletes all temporary products from the TempProducts table in the database.
        /// </summary>
        public void DeleteTempProduct()
        {
            var db = new BoulevardDbContext();
            db.TempProducts.RemoveRange(db.TempProducts);
            db.SaveChanges();
        }

        public bool AddProduct(int feacherCategoryId)
        {
            try
            {
                var db = new BoulevardDbContext();
                var tempProduct = db.TempProducts.Where(t => t.FeatureCategoryId == feacherCategoryId).ToList();

                if (tempProduct.Any())
                {
                    foreach (var item in tempProduct)
                    {
                        try
                        {
                            // Normalise both , and ; as valid delimiters throughout
                            string[] SplitValues(string raw)
                                => (raw ?? "").Replace(';', ',').Split(new[] { ',' }, StringSplitOptions.None);

                            //Brand
                            var brand = db.Brands.FirstOrDefault(b => b.Title.Trim().ToLower() == item.Brand.Trim().ToLower() && b.FeatureCategoryId == feacherCategoryId);
                            if (brand == null)
                            {
                                brand = new Brand();
                                brand.BrandKey = Guid.NewGuid();
                                brand.CreateBy = 1;
                                brand.CreateDate = DateTimeHelper.DubaiTime();
                                brand.Status = "Active";
                                brand.FeatureCategoryId = feacherCategoryId;
                                brand.Title = item.Brand.Trim();
                                brand.TitleAr = item.BrandArabic.Trim();
                                brand.Details = item.Brand.Trim();
                                db.Brands.Add(brand);
                                db.SaveChanges();
                            }

                            // ── Splitting helpers ────────────────────────────────────────────────────
                            // SplitDedup  : for Category / SubCategory — deduplicate so "A,A" → ["A"]
                            // SplitRaw    : for SubSubCategory / MiniCategory / all Arabic arrays —
                            //               NO dedup so "A,A" → ["A","A"], enabling two separate DB records.
                            string[] SplitDedup(string raw)
                                => (raw ?? "").Replace(';', ',').Replace('،', ',')
                                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(s => s.Trim()).Where(s => s.Length > 0)
                                    .Distinct(StringComparer.OrdinalIgnoreCase).ToArray();

                            string[] SplitRaw(string raw)
                                => (raw ?? "").Replace(';', ',').Replace('،', ',')
                                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(s => s.Trim()).Where(s => s.Length > 0)
                                    .ToArray();

                            string GetArAt(string[] arr, int idx) => idx < arr.Length ? arr[idx] : "";

                            // ── GetOrCreateCat: standard (no duplicate records) ──────────────────────
                            Category GetOrCreateCat(string name, string nameAr, int parentId, string imgFile)
                            {
                                var c = db.Categories.FirstOrDefault(x =>
                                    x.CategoryName.Trim().ToLower() == name.ToLower() &&
                                    x.FeatureCategoryId == feacherCategoryId &&
                                    x.ParentId == parentId);
                                if (c == null)
                                {
                                    c = new Category
                                    {
                                        CategoryKey       = Guid.NewGuid(),
                                        CreateBy          = 1,
                                        CreateDate        = DateTimeHelper.DubaiTime(),
                                        FeatureCategoryId = feacherCategoryId,
                                        CategoryName      = name.Trim(),
                                        CategoryNameAr    = (nameAr ?? "").Trim(),
                                        ParentId          = parentId,
                                        Status            = "Active"
                                    };
                                    if (!string.IsNullOrEmpty(imgFile))
                                        c.Image = "/Content/Upload/Category/" + imgFile;
                                    db.Categories.Add(c);
                                    db.SaveChanges();
                                }
                                return c;
                            }

                            // ── GetOrCreateCatTracked: allows intentional duplicate-named records ────
                            // tracker maps "name|parentId" → how many have already been claimed this
                            // import pass.  The Nth call skips the first N-1 existing records and either
                            // returns the Nth existing one or creates a brand-new record.
                            // This lets "Moisturizer, Moisturizer" produce two SEPARATE category rows.
                            Category GetOrCreateCatTracked(
                                string name, string nameAr, int parentId, string imgFile,
                                System.Collections.Generic.Dictionary<string, int> tracker)
                            {
                                string key = name.Trim().ToLower() + "|" + parentId;
                                int skip = tracker.ContainsKey(key) ? tracker[key] : 0;
                                tracker[key] = skip + 1;

                                var existing = db.Categories
                                    .Where(x => x.CategoryName.Trim().ToLower() == name.Trim().ToLower() &&
                                                x.FeatureCategoryId == feacherCategoryId &&
                                                x.ParentId == parentId &&
                                                x.Status == "Active")
                                    .OrderBy(x => x.CategoryId)
                                    .Skip(skip)
                                    .FirstOrDefault();

                                if (existing != null) return existing;

                                var c = new Category
                                {
                                    CategoryKey       = Guid.NewGuid(),
                                    CreateBy          = 1,
                                    CreateDate        = DateTimeHelper.DubaiTime(),
                                    FeatureCategoryId = feacherCategoryId,
                                    CategoryName      = name.Trim(),
                                    CategoryNameAr    = (nameAr ?? "").Trim(),
                                    ParentId          = parentId,
                                    Status            = "Active"
                                };
                                if (!string.IsNullOrEmpty(imgFile))
                                    c.Image = "/Content/Upload/Category/" + imgFile;
                                db.Categories.Add(c);
                                db.SaveChanges();
                                return c;
                            }

                            // Collect every CategoryId the product must be linked to (all levels, all branches)
                            var allCatIds = new System.Collections.Generic.HashSet<int>();

                            // Category + SubCategory: deduplicate (same name = same category)
                            var catNames      = SplitDedup(item.Category);
                            var catArNames    = SplitRaw(item.CategoryArabic);
                            var subNames      = SplitDedup(item.SubCategory);
                            var subArNames    = SplitRaw(item.SubCategoryArabic);
                            // SubSubCategory + MiniCategory: NO dedup — "A,A" creates TWO separate records
                            var subSubNames   = SplitRaw(item.SubSubCategory);
                            var subSubArNames = SplitRaw(item.SubSubCategoryArabic);
                            var miniNames     = SplitRaw(item.MiniCategory);
                            var miniArNames   = SplitRaw(item.MiniCategoryArabic);

                            // Build every branch of the 4-level hierarchy and collect all involved CategoryIds
                            foreach (var catTuple in catNames.Select((n, i) => new { n, i }))
                            {
                                var catObj = GetOrCreateCat(catTuple.n, GetArAt(catArNames, catTuple.i), 0, item.CategoryImage);
                                allCatIds.Add(catObj.CategoryId);

                                if (subNames.Length > 0)
                                {
                                    foreach (var subTuple in subNames.Select((n, i) => new { n, i }))
                                    {
                                        var subObj = GetOrCreateCat(subTuple.n, GetArAt(subArNames, subTuple.i), catObj.CategoryId, item.SubCategoryImage);
                                        allCatIds.Add(subObj.CategoryId);

                                        if (subSubNames.Length > 0)
                                        {
                                            // Fresh tracker per (cat, sub) parent so "Moisturizer,Moisturizer"
                                            // under Face creates two separate records independent of Body.
                                            var ssDupeTracker = new System.Collections.Generic.Dictionary<string, int>();
                                            foreach (var ssTuple in subSubNames.Select((n, i) => new { n, i }))
                                            {
                                                var ssObj = GetOrCreateCatTracked(ssTuple.n, GetArAt(subSubArNames, ssTuple.i), subObj.CategoryId, item.SubSubCategoryImage, ssDupeTracker);
                                                allCatIds.Add(ssObj.CategoryId);

                                                if (miniNames.Length > 0)
                                                {
                                                    // Fresh tracker per ssObj parent
                                                    var miniDupeTracker = new System.Collections.Generic.Dictionary<string, int>();
                                                    foreach (var mTuple in miniNames.Select((n, i) => new { n, i }))
                                                    {
                                                        var mObj = GetOrCreateCatTracked(mTuple.n, GetArAt(miniArNames, mTuple.i), ssObj.CategoryId, null, miniDupeTracker);
                                                        allCatIds.Add(mObj.CategoryId);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            // Find existing product — if it already exists we ONLY add new category links.
                            // The product itself (prices, images, stock) is NEVER duplicated.
                            var product = db.Products.FirstOrDefault(p =>
                                p.ProductName.Trim().ToLower() == item.ProductName.Trim().ToLower() &&
                                p.FeatureCategoryId == feacherCategoryId &&
                                p.Status == "Active");
                            bool isNewProduct = (product == null);

                            if (isNewProduct)
                            {
                            product = new Product();
                            product.ProductKey = Guid.NewGuid();
                            product.CreateBy = 1;
                            product.CreateDate = DateTimeHelper.DubaiTime();
                            product.Status = "Active";
                            product.FeatureCategoryId = feacherCategoryId;
                            product.ProductName = item.ProductName;
                            product.ProductNameAr = item.ProductNameArabic;
                            product.ProductSlag = item.ProductName;
                            product.ProductDescription = item.ItemDesc;
                            product.ProductDescriptionAr = item.ItemDescArabic;
                            product.BrandId = brand.BrandId;
                            product.DeliveryInfo = item.DeliveryInfo;
                            product.DeliveryInfoArabic = item.DeliveryInfoArabic;
                            product.Barcode = item.Barcode;
                            product.AttributeCode = item.AttributeCode;
                            product.AttributeName = item.AttributeName;
                            product.AttributeNameArabic = item.AttributeNameArabic;
                            product.IcvBoulevardScore = item.IcvBoulevardScore;

                            // C4 fix: Stocks may be comma/semicolon-separated — take the total sum for the product-level stock
                            var stockParts = SplitValues(item.Stocks);
                            product.StockQuantity = stockParts
                                .Where(s => !string.IsNullOrWhiteSpace(s) && int.TryParse(s.Trim(), out _))
                                .Sum(s => int.Parse(s.Trim()));

                            if (item.ProductType != null && item.ProductType.ToLower() == "now")
                            {
                                product.ProductType = 1;
                            }
                            else if (item.ProductType != null && item.ProductType.ToLower() == "scheduled")
                            {
                                product.ProductType = 2;
                            }
                            else
                            {
                                product.ProductType = 3;
                            }
                            db.Products.Add(product);
                            db.SaveChanges();


                            //ProductImage
                            if (!string.IsNullOrEmpty(item.Images))
                            {
                                var image = item.Images.Split(',');
                                foreach (var item1 in image)
                                {
                                    if (!string.IsNullOrEmpty(item1))
                                    {
                                        var productImage = new ProductImage
                                        {
                                            ProductId = product.ProductId,
                                            Image = "/Content/Upload/Product/" + item1,
                                            IsFeature = true,
                                        };
                                        db.ProductImages.Add(productImage);
                                        db.SaveChanges();
                                    }
                                }
                            }

                            } // end isNewProduct — product row, images created above

                            // Always update IcvBoulevardScore when a non-empty value is provided —
                            // this covers re-imports of EXISTING products that previously had no ICV data.
                            if (!string.IsNullOrWhiteSpace(item.IcvBoulevardScore))
                            {
                                product.IcvBoulevardScore = item.IcvBoulevardScore;
                                db.SaveChanges();
                            }

                            // Always run (new or existing): link product to ALL category nodes collected above.
                            // The Any() check prevents duplicate ProductCategory rows.
                            // Link product to ALL category nodes collected above (all levels, all branches).
                            foreach (var cid in allCatIds)
                            {
                                if (!db.ProductCategories.Any(pc => pc.ProductId == product.ProductId && pc.CategoryId == cid))
                                {
                                    db.ProductCategories.Add(new ProductCategory
                                        { ProductId = product.ProductId, CategoryId = cid, Status = "Active" });
                                    db.SaveChanges();
                                }
                            }

                            //Product Price — only insert for brand-new products. Re-imports reuse existing prices.
                            if (isNewProduct)
                            {
                            //Product Price — support both , and ; as delimiters
                            var qtys         = SplitValues(item.Quantity);
                            var Prices       = SplitValues(item.SellingPrice);
                            var stockquantity = SplitValues(item.Stocks);
                            // Guard: make all arrays the same length (use shortest) to prevent index out-of-bounds
                            int priceRowCount = Math.Min(qtys.Length, Math.Min(Prices.Length, stockquantity.Length));
                            for (int i = 0; i < priceRowCount; i++)
                            {
                                // Skip rows where both quantity and price are blank
                                if (string.IsNullOrWhiteSpace(qtys[i]) && string.IsNullOrWhiteSpace(Prices[i]))
                                    continue;

                                var productPrice = new ProductPrice
                                {
                                    ProductId = product.ProductId,
                                    Price = !string.IsNullOrWhiteSpace(Prices[i]) &&
                                            double.TryParse(Prices[i].Trim(), System.Globalization.NumberStyles.Any,
                                                            System.Globalization.CultureInfo.InvariantCulture, out double parsedPrice)
                                            ? parsedPrice : 0,
                                    ProductQuantity = double.TryParse((qtys[i] ?? "").Trim(), System.Globalization.NumberStyles.Any,
                                                        System.Globalization.CultureInfo.InvariantCulture, out double parsedQty) ? parsedQty : 0,
                                    ProductStock = int.TryParse((stockquantity[i] ?? "").Trim(), out int parsedStock) ? parsedStock : 0,
                                    Status = "Active",
                                    LastUpdateDate = DateTimeHelper.DubaiTime(),
                                };
                                db.ProductPrices.Add(productPrice);
                                db.SaveChanges();

                                var model = new StockLog();
                                model.StockKey = Guid.NewGuid();
                                model.ProductId = productPrice.ProductId;
                                model.StockDate = DateTimeHelper.DubaiTime();
                                model.StockIn = productPrice.ProductStock;
                                model.StockOut = 0;
                                model.ProductPriceId = productPrice.ProductPriceId;
                                model.CreateDate = DateTimeHelper.DubaiTime();
                                model.CreatedBy = 1;
                                model.StockType = "In";
                                model.OrderMasterId = 0;
                                model.UserType = "Admin";
                                model.FeatureCategoryId = feacherCategoryId;

                                db.StockLogs.Add(model);
                                db.SaveChanges();

                            }
                            } // end isNewProduct price/stock block
                        }
                        catch (Exception ex)
                        {

                            continue;
                        }

                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> AddTempProduct(string xmlFileData, int feacherCategoryId)
        {
            try
            {
                var db = new BoulevardDbContext();
                var doc = XDocument.Parse(xmlFileData);

                foreach (var elem in doc.Root.Elements("Product"))
                {
                    string GetAttr(string name) => elem.Attribute(name)?.Value?.Trim() ?? "";
                    int GetCount(string name) => int.TryParse(elem.Attribute(name)?.Value, out int v) ? v : 0;

                    var row = new TempProduct
                    {
                        TempId               = Guid.NewGuid(),
                        SrNo                 = GetAttr("srNo"),
                        Brand                = GetAttr("brand"),
                        BrandArabic          = GetAttr("brandArabic"),
                        Barcode              = GetAttr("barcode"),
                        Category             = GetAttr("category"),
                        SubCategory          = GetAttr("subCategory"),
                        SubSubCategory       = GetAttr("subSubCategory"),
                        CategoryArabic       = GetAttr("categoryArabic"),
                        SubCategoryArabic    = GetAttr("subCategoryArabic"),
                        SubSubCategoryArabic = GetAttr("subSubCategoryArabic"),
                        ItemDesc             = GetAttr("itemDesc"),
                        ItemDescArabic       = GetAttr("itemDescArabic"),
                        AttributeCode        = GetAttr("attributeCode"),
                        AttributeName        = GetAttr("attributeName"),
                        AttributeNameArabic  = GetAttr("attributeNameArabic"),
                        Images               = GetAttr("images"),
                        Quantity             = GetAttr("quantity"),
                        SellingPrice         = GetAttr("sellingPrice"),
                        ProductTags          = GetAttr("productTags"),
                        Stocks               = GetAttr("stocks"),
                        ProductName          = GetAttr("productName"),
                        ProductNameArabic    = GetAttr("productNameArabic"),
                        ProductType          = GetAttr("productType"),
                        DeliveryInfo         = GetAttr("deliveryInfo"),
                        DeliveryInfoArabic   = GetAttr("deliveryInfoArabic"),
                        CategoryImage        = GetAttr("categoryImage"),
                        SubCategoryImage     = GetAttr("subCategoryImage"),
                        SubSubCategoryImage  = GetAttr("subSubCategoryImage"),
                        ExcelCount           = GetCount("excelCount"),
                        // 4th-level category. Commas inside these strings are PART of the
                        // category name; they are never used as value separators here.
                        MiniCategory         = GetAttr("miniCategory"),
                        MiniCategoryArabic   = GetAttr("miniCategoryArabic"),
                        // ICV Boulevard Score (optional column AC in the Excel template).
                        IcvBoulevardScore    = GetAttr("icvBoulevardScore"),
                        FeatureCategoryId    = feacherCategoryId,
                    };
                    db.TempProducts.Add(row);
                }

                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}