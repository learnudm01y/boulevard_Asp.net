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

                            //Category
                            var category = db.Categories.FirstOrDefault(c => c.CategoryName.Trim().ToLower() == item.Category.Trim().ToLower() && c.FeatureCategoryId == item.FeatureCategoryId);
                            if (category == null)
                            {
                                category = new Category();
                                category.CategoryKey = Guid.NewGuid();
                                category.CreateBy = 1;
                                category.CreateDate = DateTimeHelper.DubaiTime();
                                category.FeatureCategoryId = feacherCategoryId;
                                category.CategoryName = item.Category.Trim();
                                category.CategoryNameAr = item.CategoryArabic.Trim();
                                category.Status = "Active";
                                category.ParentId = 0;
                                if (!string.IsNullOrEmpty(item.CategoryImage))
                                {
                                    category.Image = "/Content/Upload/Category/"  + item.CategoryImage;
                                }
                                db.Categories.Add(category);
                                db.SaveChanges();
                            }

                            //  SubCategory
                            var subcategory = db.Categories.FirstOrDefault(c => c.CategoryName.Trim().ToLower() == item.SubCategory.Trim().ToLower() && c.FeatureCategoryId == item.FeatureCategoryId && c.ParentId == category.CategoryId);
                            if (subcategory == null && !string.IsNullOrEmpty(item.SubCategory))
                            {
                                subcategory = new Category();
                                subcategory.CategoryKey = Guid.NewGuid();
                                subcategory.CreateBy = 1;
                                subcategory.CreateDate = DateTimeHelper.DubaiTime();
                                subcategory.FeatureCategoryId = feacherCategoryId;
                                subcategory.CategoryName = item.SubCategory.Trim();
                                subcategory.CategoryNameAr = item.SubCategoryArabic.Trim();
                                subcategory.ParentId = category.CategoryId;
                                if (!string.IsNullOrEmpty(item.SubCategoryImage))
                                {
                                    subcategory.Image = "/Content/Upload/Category/"  + item.SubCategoryImage;
                                }

                                subcategory.Status = "Active";
                                db.Categories.Add(subcategory);
                                db.SaveChanges();
                            }


                            //  SubCategory
                            var subSubcategory = db.Categories.FirstOrDefault(c => c.CategoryName.Trim().ToLower() == item.SubSubCategory.Trim().ToLower() && c.FeatureCategoryId == item.FeatureCategoryId && c.ParentId == subcategory.CategoryId);
                            if (subSubcategory == null && !string.IsNullOrEmpty(item.SubSubCategory))
                            {
                                subSubcategory = new Category();
                                subSubcategory.CategoryKey = Guid.NewGuid();
                                subSubcategory.CreateBy = 1;
                                subSubcategory.CreateDate = DateTimeHelper.DubaiTime();
                                subSubcategory.FeatureCategoryId = feacherCategoryId;
                                subSubcategory.CategoryName = item.SubSubCategory.Trim();
                                subSubcategory.CategoryNameAr = item.SubSubCategoryArabic.Trim();
                                subSubcategory.ParentId = subcategory.CategoryId;
                                if (!string.IsNullOrEmpty(item.SubSubCategoryImage))
                                {
                                    subSubcategory.Image = "/Content/Upload/Category/" + item.SubSubCategoryImage;
                                }

                                subSubcategory.Status = "Active";
                                db.Categories.Add(subSubcategory);
                                db.SaveChanges();
                            }


                            //Product
                            var product = new Product();
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

                            //product.StockQuantity = item.Stocks != null ? Convert.ToInt32(item.Stocks) : 0;
                            //if (item.ProductType.ToLower() == "now")
                            //{
                            //    product.ProductType = EnumHelper.GetEnumValueByName<ProductType>(item.ProductType.ToLower());
                            //}
                            //else if (item.ProductType.ToLower() == "scheduled")
                            //{
                            //    product.ProductType = EnumHelper.GetEnumValueByName<ProductType>(item.ProductType.ToLower());
                            //}
                            //else
                            //{
                            //    product.ProductType = 3;
                            //}

                            product.StockQuantity = item.Stocks != null ? Convert.ToInt32(item.Stocks) : 0;
                            if (item.ProductType.ToLower() == "now")
                            {
                                product.ProductType = 1;
                            }
                            else if (item.ProductType.ToLower() == "scheduled")
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

                            //Product Price


                            var qtys = item.Quantity.Split(',');
                            var Prices = item.SellingPrice.Split(',');
                            var stockquantity = item.Stocks.Split(',');
                            for (int i = 0; i < qtys.Length; i++)
                            {
                                var productPrice = new ProductPrice
                                {
                                    ProductId = product.ProductId,
                                    Price = Prices[i] != null &&
                                                !string.IsNullOrWhiteSpace(Prices[i].ToString()) &&
                                                double.TryParse(Prices[i].ToString(), out double parsedPrice)
                                                ? parsedPrice
                                                : 0,
                                    ProductQuantity = Convert.ToDouble(qtys[i]),
                                    ProductStock = Convert.ToInt32(stockquantity[i]),
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



                            // Product Category
                            if (category != null)
                            {
                                var productCategory = new ProductCategory();
                                productCategory.ProductId = product.ProductId;
                                productCategory.CategoryId = category.CategoryId;
                                productCategory.Status = "Active";

                                db.ProductCategories.Add(productCategory);
                                db.SaveChanges();
                            }

                            if (subcategory != null)
                            {
                                var productCategory = new ProductCategory();
                                productCategory.ProductId = product.ProductId;
                                productCategory.CategoryId = subcategory.CategoryId;
                                productCategory.Status = "Active";

                                db.ProductCategories.Add(productCategory);
                                db.SaveChanges();
                            }

                            if (subSubcategory != null)
                            {
                                var productCategory = new ProductCategory();
                                productCategory.ProductId = product.ProductId;
                                productCategory.CategoryId = subSubcategory.CategoryId;
                                productCategory.Status = "Active";

                                db.ProductCategories.Add(productCategory);
                                db.SaveChanges();
                            }
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

        public async Task<bool> AddTempProduct(string xmlFileData ,int feacherCategoryId)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["BoulevardDbContext"].ConnectionString;
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@tempProducts_xml", xmlFileData, DbType.Xml);
                    parameters.Add("@feacherCategoryId", feacherCategoryId);
                    await connection.ExecuteAsync("pr_upload_bulk_product", parameters, commandType: CommandType.StoredProcedure);

                }

                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}