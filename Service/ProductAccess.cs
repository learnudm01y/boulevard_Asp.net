using Antlr.Runtime.Misc;
using Boulevard.Areas.Admin.Data;
using Boulevard.BaseRepository;
using Boulevard.Contexts;
using Boulevard.Enum;
using Boulevard.Helper;
using Boulevard.Models;
using Boulevard.Service.Admin;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Log = Serilog.Log;
using Product = Boulevard.Models.Product;

namespace Boulevard.Service
{
    public class ProductAccess
    {
        public IUnitOfWork uow;
        private readonly ProductPriceDataAccess _productPriceDataAccess;
        private CategoryAccess _categoryAccess;
        
        private BrandAccess _brandAccess;
        private FeatureCategoryAccess _featureCategoryAccess;
        private readonly StockLogDataAccess _stockLogDataAccess;

        private const string ImagePathPrefix = "/Content/Upload/Product/";
        private static readonly string[] ValidExcelExtensions = { ".xls", ".xlsx" };

        public ProductAccess()
        {
            uow = new UnitOfWork();
            _productPriceDataAccess = new ProductPriceDataAccess();
            _stockLogDataAccess = new StockLogDataAccess();

            _brandAccess = new BrandAccess();
            _categoryAccess = new CategoryAccess();
            _featureCategoryAccess = new FeatureCategoryAccess();
        }

        /// <summary>
        /// Get All
        /// </summary>
        /// <returns></returns>
        public async Task<List<Product>> GetAll()
        {
            try
            {
                return await uow.ProductRepository.Get().Include(p => p.FeatureCategory).Where(e => e.Status.ToLower() == "active").ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }

        public async Task<List<Product>> GetAllByFCatagoryKey(string fCatagoryKey)
        {
            try
            {
                var product = new List<Product>();
                var fCatagory = await uow.FeatureCategoryRepository.Get().FirstOrDefaultAsync(a => a.FeatureCategoryKey.ToString() == fCatagoryKey);
                if (fCatagory == null)
                {
                    product = await uow.ProductRepository.Get().Where(e => e.Status.ToLower() != "Deleted").Include(b => b.Brands).OrderByDescending(a => a.ProductId).ToListAsync();
                }
                else
                {
                    product = await uow.ProductRepository.Get().Where(e => e.Status.ToLower() != "Deleted" && e.FeatureCategoryId == fCatagory.FeatureCategoryId).Include(b => b.Brands).OrderByDescending(a => a.ProductId).ToListAsync();
                }

                foreach (var item in product)
                {
                    var productPrice = await uow.ProductPriceRepository.Get().Where(a => a.ProductId == item.ProductId).Select(a => a.ProductStock).ToListAsync();
                    item.StockQuantity = productPrice != null ? productPrice.Sum() : 0;
                }
                return product;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Get By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Product> GetById(int id)
        {
            try
            {
                return await uow.ProductRepository.GetById(id);
            }
            catch (Exception ex)
            {
              //  Log.Error(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Get By key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<Product> GetByKey(Guid key)
        {
            try
            {
                Product product = await uow.ProductRepository.Get().Where(t => t.ProductKey == key).Include(p => p.Brands).Include(p => p.FeatureCategory).FirstOrDefaultAsync();
                List<int> categoryIds = await uow.ProductCategoryRepository.Get().Where(t => t.ProductId == product.ProductId).Select(p => p.CategoryId).ToListAsync();
                List<Category> Categories = await uow.CategoryRepository.Get().Where(p => categoryIds.Contains(p.CategoryId)).ToListAsync();
                product.ProductCategories = Categories;
                product.Images = await uow.ProductImageRepository.Get().Where(p => p.ProductId == product.ProductId).ToListAsync();
                var productPrices = await uow.ProductPriceRepository.Get().Where(p => p.Status == "Active" && p.ProductId == product.ProductId).ToListAsync();
                product.ProductPrices = productPrices != null ? productPrices : new List<ProductPrice>();

                var productList = await uow.ProductRepository.GetAll(s => s.Status == "Active" && s.FeatureCategoryId == product.FeatureCategoryId && s.ProductId != product.ProductId).ToListAsync();
                product.ProductList = productList != null ? productList : new List<Product>();


                foreach (var prod in product.ProductList)
                {
                    var upsellExist = uow.UpsellFeaturesRepository.IsExist(s => s.UpsellFeaturesTypeId == product.ProductId && s.RelatedFeatureId == prod.ProductId);
                    prod.IsUpsellProduct = upsellExist == true ? upsellExist : false;

                    var crosssellExist = uow.CrosssellFeaturesRepository.IsExist(s => s.CrosssellFeaturesTypeId == product.ProductId && s.RelatedFeatureId == prod.ProductId);
                    prod.IsCrosssellProduct = crosssellExist == true ? crosssellExist : false;
                }
                return product;
            }
            catch (Exception ex)
            {
               // Log.Error(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Insert
        /// </summary>
        /// <returns></returns>
        public async Task Insert(Product node)
        {
            try
            {
                //node.ProductType = (int)node.ProductTypes;
                node.ProductKey = Guid.NewGuid();
                node.CreateBy = 1;
                node.CreateDate = DateTimeHelper.DubaiTime();
                node.Status = "Active";
                node.StockQuantity = 0;
                node.ProductPrice = 0;
                Product product = await uow.ProductRepository.Add(node);
                if (node.ImageUrl.Count != 0)
                {
                    foreach (string img in node.ImageUrl)
                    {
                        ProductImage productImage = new ProductImage()
                        {
                            ProductId = product.ProductId,
                            Image = img,
                            IsFeature = true
                        };
                        await uow.ProductImageRepository.Add(productImage);
                    }
                }
                if (node.CommonProductTags.Count > 0)
                {
                    var commonProductTagDetails = new CommonProductTagDetails();
                    foreach (var commonTagId in node.CommonProductTags)
                    {
                        commonProductTagDetails.ProductId = product.ProductId;
                        commonProductTagDetails.CommonProductTagId = commonTagId;
                        commonProductTagDetails.CreatedAt = DateTime.Now;
                        await uow.CommonProductTagDetailsRepository.Add(commonProductTagDetails);
                    }
                }
                if (!string.IsNullOrEmpty(node.SelectedCategoryId))
                {
                    List<string> LeafCategories = node.SelectedCategoryId.Split(',').ToList();
                    foreach (string catId in LeafCategories)
                    {
                        int ctgId = Convert.ToInt32(catId);
                        ProductCategory productCategory = new ProductCategory()
                        {
                            CategoryId = Convert.ToInt32(ctgId),
                            ProductId = product.ProductId,
                            Status = "Active"
                        };

                        await uow.ProductCategoryRepository.Add(productCategory);
                        Category categoryNode = await uow.CategoryRepository.Get().Where(p => p.CategoryId == ctgId).FirstOrDefaultAsync();
                        while (categoryNode.ParentId != null && categoryNode.ParentId > 0)
                        {
                            ProductCategory prodCat = new ProductCategory()
                            {
                                CategoryId = categoryNode.ParentId.Value,
                                ProductId = product.ProductId,
                                Status = "Active"
                            };
                            await uow.ProductCategoryRepository.Add(prodCat);
                            categoryNode = await uow.CategoryRepository.Get().Where(p => p.CategoryId == categoryNode.ParentId).FirstOrDefaultAsync();
                        }
                    }
                }

                if (node.ProductPrices != null)
                {
                    foreach (var data in node.ProductPrices)
                    {
                        data.ProductId = product.ProductId;
                        data.ProductQuantity = data.ProductQuantity;
                        data.Price = data.Price;
                        data.ProductStock = data.ProductStock;
                        data.Status = "Active";
                        await uow.ProductPriceRepository.Add(data);
                        await _stockLogDataAccess.StockInProduct(data.ProductId, data.ProductStock, 1, data.ProductPriceId);

                    }
                }
            }
            catch (Exception ex)
            {
               // Log.Error(ex.ToString());
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <returns></returns>
        public async Task Update(Product node)
        {
            try
            {
                //node.ProductTypes = (ProductType)node.ProductType;
                //if(node.ProductTypes == null)
                //{
                //    node.ProductType = node.ProductType;
                //}
                //else
                //{
                //    node.ProductType = (int)node.ProductTypes;
                //}
                node.UpdateBy = 1;
                node.UpdateDate = DateTimeHelper.DubaiTime();
                node.Status = "Active";
                node.StockQuantity = 0;
                node.ProductPrice = 0;
                //Product product = await uow.ProductRepository.Edit(node);
                var db = new BoulevardDbContext();
                db.Entry(node).State = EntityState.Modified;
                db.SaveChanges();

                foreach (string img in node.ImageUrl)
                {
                    ProductImage productImage = new ProductImage()
                    {
                        //ProductId = product.ProductId,
                        ProductId = node.ProductId,
                        Image = img,
                        IsFeature = true
                    };
                    await uow.ProductImageRepository.Add(productImage);
                }
                if (node.CommonProductTags != null && node.CommonProductTags.Count() > 0)
                {
                    var commonNodeIds = await uow.CommonProductTagDetailsRepository.Get().Where(a => a.ProductId == node.ProductId).ToListAsync();
                    if (commonNodeIds != null)
                    {
                        foreach (var data in node.CommonProductTags)
                        {
                            foreach (var common in commonNodeIds)
                            {

                                if (data != common.CommonProductTagId)
                                {
                                    uow.CommonProductTagDetailsRepository.Remove(common);
                                }
                            }
                        }
                    }
                    var commonProductTagDetails = new CommonProductTagDetails();
                    foreach (var commonTagId in node.CommonProductTags)
                    {
                        commonProductTagDetails.ProductId = node.ProductId;
                        commonProductTagDetails.CommonProductTagId = commonTagId;
                        commonProductTagDetails.CreatedAt = DateTime.Now;
                        await uow.CommonProductTagDetailsRepository.Add(commonProductTagDetails);
                    }
                }

                List<ProductCategory> ProdCat = await uow.ProductCategoryRepository.Get().Where(p => p.ProductId == node.ProductId).ToListAsync();
                //List<ProductCategory> ProdCat = await uow.ProductCategoryRepository.Get().Where(p => p.ProductId == product.ProductId).ToListAsync();
                uow.ProductCategoryRepository.MultipleRemove(ProdCat);
                if (!string.IsNullOrEmpty(node.SelectedCategoryId))
                {
                    List<string> LeafCategories = node.SelectedCategoryId.Split(',').ToList();
                    foreach (string catId in LeafCategories)
                    {
                        int ctgId = Convert.ToInt32(catId);
                        if (await uow.ProductCategoryRepository.Get().AnyAsync(s => s.ProductId == node.ProductId && s.CategoryId == ctgId) == false)
                        {
                            ProductCategory productCategory = new ProductCategory()
                            {
                                CategoryId = Convert.ToInt32(ctgId),
                                ProductId = node.ProductId,
                                //ProductId = product.ProductId,
                                Status = "Active"
                            };
                            await uow.ProductCategoryRepository.Add(productCategory);
                        }


                        Category categoryNode = await uow.CategoryRepository.Get().Where(p => p.CategoryId == ctgId).FirstOrDefaultAsync();
                        while (categoryNode.ParentId != null && categoryNode.ParentId > 0)
                        {
                            if (await uow.ProductCategoryRepository.Get().AnyAsync(s => s.ProductId == node.ProductId && s.CategoryId == categoryNode.ParentId) == false)
                            {
                                ProductCategory prodCat = new ProductCategory()
                                {
                                    CategoryId = categoryNode.ParentId.Value,
                                    ProductId = node.ProductId,
                                    //ProductId = product.ProductId,
                                    Status = "Active"
                                };
                                await uow.ProductCategoryRepository.Add(prodCat);
                            }
                            categoryNode = await uow.CategoryRepository.Get().Where(p => p.CategoryId == categoryNode.ParentId).FirstOrDefaultAsync();
                        }
                    }
                }

                if (node.ProductPrices != null)
                {
                    List<ProductPrice> productPrices = await uow.ProductPriceRepository.Get().Where(p => p.ProductId == node.ProductId).ToListAsync();
                    //List<ProductCategory> ProdCat = await uow.ProductCategoryRepository.Get().Where(p => p.ProductId == product.ProductId).ToListAsync();
                    //uow.ProductPriceRepository.MultipleRemove(productPrices);
                    foreach (var productPrice in node.ProductPrices)
                    {
                        if (await _productPriceDataAccess.IsExisr(productPrice.ProductPriceId, productPrice.ProductId))
                        {
                            productPrice.ProductId = node.ProductId;
                            productPrice.ProductQuantity = productPrice.ProductQuantity;
                            productPrice.Price = productPrice.Price;
                            productPrice.Status = "Active";
                            //await uow.ProductPriceRepository.Edit(data);
                            //var db = new BoulevardDbContext();
                            db.Entry(productPrice).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            productPrice.ProductId = node.ProductId;
                            productPrice.ProductQuantity = productPrice.ProductQuantity;
                            productPrice.Price = productPrice.Price;
                            productPrice.Status = "Active";
                            await uow.ProductPriceRepository.Add(productPrice);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
               // Log.Error(ex.ToString());
            }
        }

        /// <summary>
        /// Post Upsell
        /// </summary>
        /// <param name="productIds"></param>
        /// <param name="productKey"></param>
        /// <returns></returns>
        public async Task PostUpsell(List<Int64> productIds, Guid productKey)
        {
            try
            {
                var db = new BoulevardDbContext();
                var productData = await GetByKey(productKey);
                var upSellList = db.UpsellFeatures.Where(s => s.UpsellFeaturesTypeId == productData.ProductId).ToList();
                if (upSellList != null)
                {
                    foreach (var upsell in upSellList)
                    {
                        db.UpsellFeatures.Attach(upsell);
                        db.UpsellFeatures.Remove(upsell);
                        db.SaveChanges();
                    }
                }
                foreach (var property in productIds)
                {
                    var result = await uow.ProductRepository.GetById(property);
                    var upsell = new UpsellFeatures();
                    upsell.UpsellFeaturesKey = Guid.NewGuid();
                    upsell.Status = "Active";
                    upsell.UpsellFeaturesType = "Product";
                    upsell.UpsellFeaturesTypeId = productData.ProductId;
                    upsell.RelatedFeatureId = result.ProductId;
                    upsell.FeatureCategoryId = result.FeatureCategoryId.Value;
                    upsell.CreateDate = DateTimeHelper.DubaiTime();
                    upsell.CreateBy = 1;
                    upsell.UpdateDate = DateTimeHelper.DubaiTime();
                    upsell.UpdateBy = 1;

                    await uow.UpsellFeaturesRepository.Add(upsell);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        /// <summary>
        /// Post Crosssell
        /// </summary>
        /// <param name="productIds"></param>
        /// <param name="productKey"></param>
        /// <returns></returns>
        public async Task PostCrosssell(List<Int64> productIds, Guid productKey)
        {
            try
            {
                var db = new BoulevardDbContext();
                var productData = await GetByKey(productKey);
                var crossSellList = db.CrosssellFeatures.Where(s => s.CrosssellFeaturesTypeId == productData.ProductId).ToList();
                if (crossSellList != null)
                {
                    foreach (var cross in crossSellList)
                    {
                        db.CrosssellFeatures.Attach(cross);
                        db.CrosssellFeatures.Remove(cross);
                        db.SaveChanges();
                    }
                }
                foreach (var property in productIds)
                {

                    var result = await uow.ProductRepository.GetById(property);

                    var upsell = new CrosssellFeatures();
                    upsell.CrosssellFeaturesKey = Guid.NewGuid();
                    upsell.Status = "Active";
                    upsell.CrosssellFeaturesType = "Product";
                    upsell.CrosssellFeaturesTypeId = productData.ProductId;
                    upsell.RelatedFeatureId = result.ProductId;
                    upsell.FeatureCategoryId = result.FeatureCategoryId.Value;
                    upsell.CreateDate = DateTimeHelper.DubaiTime();
                    upsell.CreateBy = 1;
                    upsell.UpdateDate = DateTimeHelper.DubaiTime();
                    upsell.UpdateBy = 1;

                    await uow.CrosssellFeaturesRepository.Add(upsell);

                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateby"></param>
        /// <returns></returns>
        public async Task<bool> Delete(Guid key, int updateby)
        {
            try
            {
                var exitResult = await GetByKey(key);
                if (exitResult != null)
                {
                    exitResult.DeleteBy = 1;
                    exitResult.DeleteDate = DateTimeHelper.DubaiTime();
                    exitResult.Status = "Deleted";
                    await uow.ProductRepository.Edit(exitResult);

                    if (exitResult.ProductPrices != null)
                    {
                        foreach (var data in exitResult.ProductPrices)
                        {
                            data.LastUpdateDate = DateTimeHelper.DubaiTime();
                            data.Status = "Deleted";
                            await uow.ProductPriceRepository.Edit(data);
                            //return true;
                        }
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
               // Log.Error(ex.ToString());
                return false;
            }
        }
        /// <summary>
        /// Delete Image
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateby"></param>
        /// <returns></returns>
        public async Task<bool> DeleteProductImage(int ImageId)
        {
            try
            {
                var exitResult = uow.ProductImageRepository.GetbyId(ImageId);
                if (exitResult != null)
                {
                    uow.ProductImageRepository.Remove(exitResult);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
              //  Log.Error(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// Upload Image
        /// </summary>
        /// <param name="flagImage"></param>
        /// <returns></returns>
        public string UploadImage(HttpPostedFileBase flagImage)
        {
            string ImageName = string.Empty;
            string Url = "/Content/Upload/Product";
            ImageName = MediaHelper.UploadImage(flagImage, Url);
            return ImageName;
        }
        //public async Task<(int InsertedCount, int FailedCount, string Message, string AlertType)> ProcessExcelProductUpload(
        //        HttpPostedFileBase excelFile, Guid featureCategoryKey)
        //{
        //    int insertedCount = 0;
        //    int failedCount = 0;
        //    var errors = new List<string>();

        //    // Validate input
        //    if (excelFile == null || excelFile.ContentLength <= 0)
        //    {
        //        return (0, 0, "No file uploaded or file is empty.", "danger");
        //    }


        //    if (!ValidExcelExtensions.Any(ext => excelFile.FileName.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
        //    {
        //        return (0, 0, "Please upload a valid Excel file (.xls or .xlsx).", "danger");
        //    }

        //    var featureCategory = await _featureCategoryAccess.GetByKey(featureCategoryKey);
        //    if (featureCategory == null)
        //    {
        //        return (0, 0, "Feature Category not found.", "danger");
        //    }

        //    try
        //    {
        //        using (var package = new ExcelPackage(excelFile.InputStream))
        //        {
        //            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
        //            if (worksheet == null)
        //            {
        //                return (0, 0, "No worksheet found in Excel file.", "danger");
        //            }
        //            // Process each row
        //            for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
        //            {
        //                var entry = new ProductEntryExcellUploadViewModel
        //                {
        //                    SrNo = worksheet.Cells[row, 1].Text,
        //                    Brand = worksheet.Cells[row, 2].Text,
        //                    Barcode = worksheet.Cells[row, 3].Text,
        //                    Category = worksheet.Cells[row, 4].Text,
        //                    SubCategory = worksheet.Cells[row, 5].Text,
        //                    ItemDesc = worksheet.Cells[row, 6].Text,
        //                    AttributeCode = worksheet.Cells[row, 7].Text,
        //                    AttributeName = worksheet.Cells[row, 8].Text,
        //                    Quantity = worksheet.Cells[row, 16].Text,
        //                    SellingPrice = worksheet.Cells[row, 17].Text,
        //                    SubSkuId = worksheet.Cells[row, 28].Text,
        //                    ProductTags = worksheet.Cells[row, 29].Text,
        //                    StocksQuantity = worksheet.Cells[row, 30].Text
        //                };

        //                // Collect image URLs
        //                for (int col = 9; col <= 15; col++)
        //                {
        //                    string image = worksheet.Cells[row, col].Text;
        //                    if (!string.IsNullOrWhiteSpace(image))
        //                    {
        //                        entry.ImageUrls.Add(image);
        //                    }
        //                }

        //                // Skip invalid entries
        //                if (string.IsNullOrEmpty(entry.Brand) || string.IsNullOrEmpty(entry.Category))
        //                {
        //                    failedCount++;
        //                    errors.Add($"Row {row}: Missing brand or category.");
        //                    continue;
        //                }


        //                // Initialize product
        //                var product = new Product
        //                {
        //                    ProductKey = Guid.NewGuid(),
        //                    FeatureCategoryId = featureCategory.FeatureCategoryId,
        //                    ProductName = entry.ItemDesc,
        //                    CreateBy = 1,
        //                    CreateDate = DateTimeHelper.DubaiTime(),
        //                    Status = "Active",
        //                    StockQuantity = int.TryParse(entry.StocksQuantity, out int stockQty) ? stockQty : 0,
        //                    ImageUrl = entry.ImageUrls
        //                };
        //                // Resolve brand
        //                var brand = await _brandAccess.GetByBrandName(entry.Brand);
        //                if (brand == null)
        //                {
        //                    brand = await _brandAccess.Insertv2(new Brand
        //                    {
        //                        Title = entry.Brand,
        //                        FeatureCategoryId = featureCategory.FeatureCategoryId
        //                    });
        //                }
        //                product.BrandId = brand?.BrandId ?? 0;

        //                // Resolve category
        //                var category = await _categoryAccess.GetByCategoryName(entry.Category);
        //                if (category == null)
        //                {
        //                    category = await _categoryAccess.InsertV2(new Category
        //                    {
        //                        CategoryName = entry.Category,
        //                        CategoryDescription = entry.Category,
        //                        FeatureCategoryId = featureCategory.FeatureCategoryId
        //                    });
        //                }
        //                product.CategoryId = category?.CategoryId ?? 0;

        //                // Resolve subcategory
        //                if (!string.IsNullOrWhiteSpace(entry.SubCategory) && product.CategoryId > 0)
        //                {
        //                    var subCategory = await _categoryAccess.GetBySubCategoryName(entry.SubCategory);
        //                    if (subCategory == null)
        //                    {
        //                        subCategory = await _categoryAccess.InsertV2(new Category
        //                        {
        //                            CategoryName = entry.SubCategory,
        //                            CategoryDescription = entry.SubCategory,
        //                            FeatureCategoryId = featureCategory.FeatureCategoryId,
        //                            ParentId = product.CategoryId
        //                        });
        //                    }
        //                    product.SubCategoryId = subCategory?.CategoryId ?? 0;
        //                }

        //                // Parse product prices
        //                var priceMap = new (string QtyText, string PriceText)[]
        //                {
        //                            (entry.FirstQuantity, entry.FirstSellingPrice),
        //                            (entry.SecondQuantity, entry.SecondSellingPrice),
        //                            (entry.ThirdQuantity, entry.ThirdSellingPrice),
        //                            (entry.FourthQuantity, entry.FourthSellingPrice),
        //                            (entry.FifthQuantity, entry.FifthSellingPrice),
        //                            (entry.SixthQuantity, entry.SixthSellingPrice)
        //                };

        //                var productPrices = new List<ProductPrice>();
        //                foreach (var (qtyText, priceText) in priceMap)
        //                {
        //                    var match = Regex.Match(qtyText, @"\d+");
        //                    if (match.Success &&
        //                        int.TryParse(match.Value, out int quantity) &&
        //                        double.TryParse(priceText, out double price))
        //                    {
        //                        productPrices.Add(new ProductPrice
        //                        {
        //                            ProductQuantity = quantity,
        //                            Price = price,
        //                            LastUpdateDate = DateTimeHelper.DubaiTime(),
        //                            Status = "Active"
        //                        });
        //                    }
        //                }
        //                product.ProductPrices = productPrices;

        //                // Insert product
        //                var insertedProduct = await uow.ProductRepository.Add(product);
        //                if (insertedProduct == null || insertedProduct.ProductId <= 0)
        //                {
        //                    failedCount++;
        //                    errors.Add($"Row {row}: Failed to insert product {entry.ItemDesc}.");
        //                    continue;
        //                }

        //                // Add product images
        //                foreach (var img in product.ImageUrl)
        //                {
        //                    await uow.ProductImageRepository.Add(new ProductImage
        //                    {
        //                        ProductId = insertedProduct.ProductId,
        //                        Image = $"{ImagePathPrefix}{img}",
        //                        IsFeature = true
        //                    });
        //                }

        //                // Add product category
        //                if (product.CategoryId > 0)
        //                {
        //                    bool existsCtgry = uow.ProductCategoryRepository.IsExist(
        //                        e => e.ProductId == insertedProduct.ProductId && e.CategoryId == product.CategoryId);

        //                    if (!existsCtgry)
        //                    {
        //                        await uow.ProductCategoryRepository.Add(new ProductCategory
        //                        {
        //                            CategoryId = product.CategoryId ?? 0,
        //                            ProductId = insertedProduct.ProductId,
        //                            Status = "Active"
        //                        });

        //                    }
        //                    else
        //                    {

        //                    }
        //                }

        //                // Add product subcategory
        //                if (product.SubCategoryId > 0)
        //                {
        //                    bool existsSubCtgry = uow.ProductCategoryRepository.IsExist(
        //                        e => e.ProductId == insertedProduct.ProductId && e.CategoryId == product.SubCategoryId);
        //                    if (!existsSubCtgry)
        //                    {
        //                        await uow.ProductCategoryRepository.Add(new ProductCategory
        //                        {
        //                            CategoryId = product.SubCategoryId ?? 0,
        //                            ProductId = insertedProduct.ProductId,
        //                            Status = "Active"
        //                        });
        //                    }
        //                }

        //                // Add stock
        //                if (product.StockQuantity > 0)
        //                {
        //                    await _stockLogDataAccess.StockIn(insertedProduct.ProductId, product.StockQuantity, 1);
        //                }

        //                // Add product prices
        //                foreach (var price in productPrices)
        //                {
        //                    price.ProductId = insertedProduct.ProductId;
        //                    await uow.ProductPriceRepository.Add(price);
        //                }

        //                insertedCount++;
        //            }
        //        }

        //        // Construct result message
        //        string message = errors.Any()
        //            ? $"Excel file uploaded. ✅ {insertedCount} rows inserted, ❌ {failedCount} rows failed: {string.Join("; ", errors)}"
        //            : $"Excel file uploaded. ✅ {insertedCount} rows successfully inserted.";
        //        string alertType = errors.Any() ? "danger" : "success";

        //        return (insertedCount, failedCount, message, alertType);
        //    }
        //    catch (Exception ex)
        //    {
        //       // Log.Error(ex.ToString());
        //        return (0, 0, "An error occurred while processing the Excel file.", "danger");
        //    }
        //}
        
    }
}