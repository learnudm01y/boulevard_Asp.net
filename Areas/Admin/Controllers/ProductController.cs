using Boulevard.Areas.Admin.Data;
using Boulevard.Contexts;
using Boulevard.Enum;
using Boulevard.Helper;
using Boulevard.Models;
using Boulevard.Service;
using Boulevard.Service.Admin;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Windows.Media.Media3D;
using System.Xml;


namespace Boulevard.Areas.Admin.Controllers
{
    public class ProductController : BaseController
    {
        private CategoryAccess _categoryAccess;
        private ProductAccess _productAccess;
        private BrandAccess _brandAccess;
        private FeatureCategoryAccess _featureCategoryAccess;
        private readonly CommonProductTagDataAcces _commonProductTagDataAcces;
        private readonly StockLogDataAccess _stockLogDataAccess;
        private readonly ProductPriceDataAccess _productPriceDataAccess;
        private readonly TempProductDataAccess _tempMemberDataAccess;
        private BoulevardDbContext _context;

        public ProductController()
        {
            _brandAccess = new BrandAccess();
            _productAccess = new ProductAccess();
            _categoryAccess = new CategoryAccess();
            _featureCategoryAccess = new FeatureCategoryAccess();
            _commonProductTagDataAcces = new CommonProductTagDataAcces();
            _stockLogDataAccess = new StockLogDataAccess();
            _productPriceDataAccess = new ProductPriceDataAccess();
            _tempMemberDataAccess = new TempProductDataAccess();
            _context = new BoulevardDbContext();
        }
        public async Task<ActionResult> Index(string fCatagoryKey)
        {
            ViewBag.FCatagoryKey = fCatagoryKey;
            if (!string.IsNullOrEmpty(fCatagoryKey))
            {
                ViewBag.FCatagoryName = await _featureCategoryAccess.GetFeatureCategoryName(fCatagoryKey);
            }
            return View();
        }

        // Returns COUNT only – called deferred after rows are already rendered.
        [HttpGet]
        public async Task<ActionResult> GetPagedProductsCount(string fCatagoryKey, string searchTerm)
        {
            try
            {
                var query = _context.Products.Where(p => p.Status == "Active");

                if (!string.IsNullOrEmpty(fCatagoryKey) && Guid.TryParse(fCatagoryKey, out Guid fcGuid))
                {
                    var fcId = await _context.featureCategories
                        .Where(f => f.FeatureCategoryKey == fcGuid)
                        .Select(f => f.FeatureCategoryId)
                        .FirstOrDefaultAsync();
                    if (fcId > 0)
                        query = query.Where(p => p.FeatureCategoryId == fcId);
                }

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var term = searchTerm.Trim().ToLower();
                    query = query.Where(p => p.ProductName.ToLower().Contains(term));
                }

                int total = await query.CountAsync();
                return Json(new { success = true, total }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.ToString());
                return Json(new { success = false, total = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetPagedProducts(string fCatagoryKey, string searchTerm, int page = 1, int pageSize = 20)
        {
            try
            {
                if (pageSize < 10 || pageSize > 500) pageSize = 20;
                if (page < 1) page = 1;

                var query = _context.Products.Where(p => p.Status == "Active");

                if (!string.IsNullOrEmpty(fCatagoryKey) && Guid.TryParse(fCatagoryKey, out Guid fcGuid))
                {
                    var fcId = await _context.featureCategories
                        .Where(f => f.FeatureCategoryKey == fcGuid)
                        .Select(f => f.FeatureCategoryId)
                        .FirstOrDefaultAsync();
                    if (fcId > 0)
                        query = query.Where(p => p.FeatureCategoryId == fcId);
                }

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var term = searchTerm.Trim().ToLower();
                    query = query.Where(p => p.ProductName.ToLower().Contains(term));
                }

                var rows = await query
                    .OrderByDescending(p => p.ProductId)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new
                    {
                        productKey = p.ProductKey,
                        productName = p.ProductName,
                        productPrice = p.ProductPrice,
                        stockQuantity = p.StockQuantity,
                        productDescription = p.ProductDescription,
                        deliveryInfo = p.DeliveryInfo,
                        brandTitle = p.Brands == null ? "N/A" : p.Brands.Title
                    })
                    .ToListAsync();

                return Json(new { success = true, rows }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.ToString());
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public async Task<ActionResult> Create(Guid? Key, string fCatagoryKey)
        {
            var featureCategories = new SelectList(await _featureCategoryAccess.GetAllByFCatagoryKey(fCatagoryKey), "FeatureCategoryId", "Name");
            if (featureCategories != null)
            {
                ViewBag.featureCategory = featureCategories;
            }
            var brandList = new SelectList(await _brandAccess.GetAllByFeatureCategory(fCatagoryKey), "BrandId", "Title");
            if (brandList != null)
            {
                ViewBag.brand = brandList;
            }
            //ViewBag.TagName = new SelectList(await _commonProductTagDataAcces.GetAllByFCatagoryKey(fCatagoryKey), "CommonProductTagId", "TagName");
            var tagName = await _commonProductTagDataAcces.GetAllByFCatagoryKey(fCatagoryKey);
            if (tagName != null)
            {
                ViewBag.TagName = tagName;
            }
            List<Category> AllCategories = await _categoryAccess.GetAllByFeatureCategory(fCatagoryKey);

            //ViewBag.ProductType = new SelectList(Enum.ProductType.GetValues(typeof(ProductType)));
            var productTypes = new SelectList(await _stockLogDataAccess.GetAllProductType(), "ProductTypeId", "Name");
            if (productTypes != null)
            {
                ViewBag.ProductType = productTypes;
            }
            if (Key == null || Key == Guid.Empty)
            {
                ViewBag.FeatureCategoryKey = fCatagoryKey;
                if (!string.IsNullOrEmpty(fCatagoryKey))
                {
                    ViewBag.FCatagoryName = await _featureCategoryAccess.GetFeatureCategoryName(fCatagoryKey);
                }
                Models.Product node = new Models.Product();
                if (AllCategories != null)
                {
                    node.CategoryTree = await _categoryAccess.GetParentChildWiseCategories(AllCategories);
                }
                //if (node.ProductPrices == null)
                //{
                //    node.ProductPrices.Add(new ProductPrice());
                //}
                return View(node);
            }
            else
            {
                var db = new BoulevardDbContext();
                Models.Product node = await _productAccess.GetByKey(Key.Value);
                //node.ProductTypes = (ProductType)node.ProductType;
                FeatureCategory featureCategory = await _featureCategoryAccess.GetById(node.FeatureCategoryId.Value);
                if (!string.IsNullOrEmpty(featureCategory.FeatureCategoryKey.ToString()))
                {
                    ViewBag.FCatagoryName = await _featureCategoryAccess.GetFeatureCategoryName(featureCategory.FeatureCategoryKey.ToString());
                    ViewBag.FeatureCategoryKey = featureCategory.FeatureCategoryKey;
                }
                var commonTag = db.CommonProductTagDetails.Where(a => a.ProductId == node.ProductId).Select(s => s.CommonProductTagId).ToList();
                if (commonTag.Count > 0)
                {
                    node.CommonProductTags = commonTag;
                }
                if (AllCategories != null)
                {
                    node.CategoryTree = await _categoryAccess.GetParentChildWiseCategories(AllCategories);
                }

                if (node.ProductPrices != null)
                {
                    node.ProductPrices = await _productPriceDataAccess.GetAllByProductId(node.ProductId);
                }
                return View(node);
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(Models.Product model, IEnumerable<HttpPostedFileBase> images)
        {
            if (images != null)
            {
                foreach (var img in images)
                {
                    if (img != null)
                        model.ImageUrl.Add(_productAccess.UploadImage(img));
                }
            }
            if (model.ProductKey == Guid.Empty)
            {
                var featureCategory = new FeatureCategory();
                if (!string.IsNullOrEmpty(model.FeatureCategoryKey))
                {
                    featureCategory = await _featureCategoryAccess.GetByKey(Guid.Parse(model.FeatureCategoryKey));
                    model.FeatureCategoryId = featureCategory.FeatureCategoryId;
                }
                await _productAccess.Insert(model);

            }
            else
            {
                var result = await _productAccess.GetById(model.ProductId);
                model.FeatureCategoryId = result.FeatureCategoryId;
                await _productAccess.Update(model);
            }
            if (!string.IsNullOrEmpty(model.FeatureCategoryKey))
            {
                return RedirectToAction("Index", "Product", new { fCatagoryKey = model.FeatureCategoryKey });
            }
            else
            {
                return RedirectToAction("Index", "Product");
            }
        }
        public async Task<ActionResult> UpsellProduct(Guid Key)
        {
            try
            {
                Models.Product list = await _productAccess.GetByKey(Key);
                return View(list);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.ToString());
                return null;
            }
        }

        [HttpPost]
        public async Task<JsonResult> PostUpsell(List<Int64> productIds, Guid Key)
        {
            try
            {
                await _productAccess.PostUpsell(productIds, Key);

                return Json(new { success = true, responseText = "Successfull" });
            }
            catch (Exception)
            {
                return Json(new { success = false, responseText = "Failed" });
            }
        }

        public async Task<ActionResult> CrosssellProduct(Guid Key)
        {
            try
            {
                Models.Product list = await _productAccess.GetByKey(Key);
                return View(list);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.ToString());
                return null;
            }
        }

        [HttpPost]
        public async Task<JsonResult> PostCrosssell(List<Int64> productIds, Guid Key)
        {
            try
            {
                await _productAccess.PostCrosssell(productIds, Key);

                return Json(new { success = true, responseText = "Successfull" });
            }
            catch (Exception)
            {
                return Json(new { success = false, responseText = "Failed" });
            }
        }
        [HttpGet]
        public async Task<bool> Delete(Guid? Key)
        {
            if (Key == null || Key == Guid.Empty)
            {
                return false;
            }
            else
            {
                return await _productAccess.Delete(Key.Value, 1);
            }
        }
        public async Task<ActionResult> Details(Guid Key)
        {
            Models.Product list = await _productAccess.GetByKey(Key);
            return View(list);
        }
        [HttpGet]
        public async Task<bool> DeleteImage(int ImageId)
        {

            return await _productAccess.DeleteProductImage(ImageId);

        }

        public async Task<ActionResult> StockLogDetails(Guid Key)
        {
            try
            {
                ViewBag.AdminId = GetUser().UserId;
                Models.Product product = await _productAccess.GetByKey(Key);
                if (product != null)
                {
                    product.StockLogList = await _stockLogDataAccess.GetAllStockLogByProductId(product.ProductId);
                }
                if (product.ProductId > 0)
                {
                    await LoadDdl(product.ProductId);
                }
                return View(product);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.ToString());
                return null;
            }
        }

        [HttpPost]
        public async Task<ActionResult> StockIn(int productId, int stockin, int adminId, int ProductPriceId)
        {
            try
            {
                var db = new BoulevardDbContext();
                var productKey = await _productAccess.GetById(productId);
                await _stockLogDataAccess.StockIn(productId, stockin, adminId, ProductPriceId);


                return RedirectToAction("StockLogDetails", "Product", new { Key = productKey.ProductKey });
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        public async Task<ActionResult> StockOut(int productId, int stockin, int adminId, int ProductPriceId)
        {
            try
            {
                var db = new BoulevardDbContext();
                var productKey = await _productAccess.GetById(productId);
                await _stockLogDataAccess.StockOut(productId, 0, stockin, adminId, ProductPriceId);


                return RedirectToAction("StockLogDetails", "Product", new { Key = productKey.ProductKey });
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        public async Task<bool> DeleteProductPrice(int productPriceId)
        {
            if (productPriceId > 0)
            {
                return await _productPriceDataAccess.Delete(productPriceId);
            }
            else
            {
                return false;
            }
        }


        #region Drop Down

        private async Task LoadDdl(int productId)
        {
            var db = new BoulevardDbContext();
            var productPrice = await db.ProductPrices.Where(a => a.ProductId == productId).ToListAsync();
            var productName = await db.Products.Where(a => a.ProductId == productId).Select(a => a.ProductName).FirstOrDefaultAsync();
            List<SelectListItem> selectProductPrice = productPrice.Select(l => new SelectListItem
            {
                Value = l.ProductPriceId.ToString(),
                Text = productName + " " + "(" + " " + l.Price.ToString() + " " + "AED" + ")",
                //Text = $"{productName} ({l.Price} AED)"

            }).ToList();
            ViewBag.ProductPrice = selectProductPrice;
        }

        #endregion

        #region Test Task
        public ActionResult GroceryTestIndex()
        {
            return View();
        }
        public ActionResult GroceryTestAdd()
        {
            return View();
        }
        public ActionResult GroceryTestEdit()
        {
            return View();
        }
        public ActionResult GroceryTestDetails()
        {
            return View();
        }

        public ActionResult ChocolateAndFlawerTestIndex()
        {
            return View();
        }
        public ActionResult ChocolateAndFlawerTestAdd()
        {
            return View();
        }
        public ActionResult ChocolateAndFlawerTestEdit()
        {
            return View();
        }
        public ActionResult ChocolateAndFlawerTestDetails()
        {
            return View();
        }
        #endregion


        public async Task<ActionResult> GetProductByFeatureCategory(string fCatagoryKey)
        {
            if (string.IsNullOrEmpty(fCatagoryKey))
            {
                return Json(new { success = false, message = "Feature Category Key is required." }, JsonRequestBehavior.AllowGet);
            }
            var products = await _productAccess.GetAllByFCatagoryKey(fCatagoryKey);
            return Json(new { success = true, data = products }, JsonRequestBehavior.AllowGet);
        }



        #region Bulk Upload
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> AddBulk(string message = "", string fCatagoryKey="")
        {
            var tempProduct = _tempMemberDataAccess.GetTempProductCount();
            ViewBag.NewRecord = tempProduct.TotalCount;
            ViewBag.Message = message;
            ViewBag.FCatagoryKey = fCatagoryKey;
            tempProduct.fCatagoryKey = fCatagoryKey;
            return View(tempProduct);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddBulk(TempProductCountViewModel model)
        {
            var Password = Request.Form["Password"] ?? "";
            _tempMemberDataAccess.DeleteTempProduct();
            if (Request.Files.Count > 0)
            {
                try
                {
                    var feacherCategory =await _featureCategoryAccess.GetByKey(Guid.Parse(model.fCatagoryKey));

                    StringBuilder queryString = new StringBuilder();
                    DataSet ds = new DataSet();
                    DataTable dt = new DataTable();
                    int readDone = 0;
                    if (Request.Files.Count > 0)
                    {
                        var file1 = Request.Files[0];
                        string fileExtension = Path.GetExtension(file1.FileName);

                        if (fileExtension == ".xls" || fileExtension == ".xlsx")
                        {
                            string rootpath = "/Content/uploads/ExcelFiles";

                            var generator = new Random();
                            var randKey = generator.Next(0, 1000000).ToString("D6");
                            var fileName = randKey + "_image_" + Regex.Replace(file1.FileName, @"\s+", "");
                            var path = Path.Combine(Server.MapPath("~" + rootpath), fileName);

                            if (!Directory.Exists(Server.MapPath("~" + rootpath)))
                            {
                                Directory.CreateDirectory(Server.MapPath("~" + rootpath));
                            }
                            file1.SaveAs(path);
                            string filePath = rootpath + fileName;

                            string excelConnectionString = string.Empty;
                            if (!string.IsNullOrEmpty(Password))
                            {
                                excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Password=" + Password + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                            }
                            else
                            {
                                excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                            }

                            OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                            excelConnection.Open();

                            dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            if (dt == null)
                            {
                                return RedirectToAction(nameof(AddBulk), new { message = "No File Found.", fCatagoryKey = feacherCategory.FeatureCategoryKey.ToString() });
                            }

                            String[] excelSheets = new String[dt.Rows.Count];
                            int t = 0;
                            //excel data saves in temp file here.
                            foreach (DataRow row in dt.Rows)
                            {
                                excelSheets[t] = row["TABLE_NAME"].ToString();
                                t++;
                            }

                            OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);

                            string query = string.Format("Select * from [{0}]", excelSheets[0]);
                            using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                            {
                                dataAdapter.Fill(ds);
                                excelConnection.Close();
                            }

                            var dataTable = ds.Tables[0];

                            #region Check Excel Column
                            TempProduct product = new TempProduct();
                            //product.SrNo = dataTable.Rows[0]["Sr.No"].ToString();
                            product.Brand = dataTable.Rows[0]["Brand"].ToString();
                            product.Barcode = dataTable.Rows[0]["Barcode"].ToString();
                            product.Category = dataTable.Rows[0]["Category"].ToString();
                            product.SubCategory = dataTable.Rows[0]["Sub Category"].ToString();
                            product.SubSubCategory = dataTable.Rows[0]["Sub Sub Category"].ToString();
                            product.ItemDesc = dataTable.Rows[0]["Item Desc"].ToString();
                            product.AttributeCode = dataTable.Rows[0]["Attribute Code"].ToString();
                            product.AttributeName = dataTable.Rows[0]["Attribute Name"].ToString();
                            product.Images = dataTable.Rows[0]["images"].ToString();
                            product.Quantity = dataTable.Rows[0]["Quantitys"].ToString();
                            product.SellingPrice = dataTable.Rows[0]["Selling Price"].ToString();
                            product.ProductTags = dataTable.Rows[0]["Product Tags"].ToString();
                            product.Stocks = dataTable.Rows[0]["Stocks quantity"].ToString();
                            product.ProductName = dataTable.Rows[0]["ProductName"].ToString();
                            product.ProductType = dataTable.Rows[0]["Product Type"].ToString();
                            product.AttributeNameArabic = dataTable.Rows[0]["Attribute Name Arabic"].ToString();
                            product.DeliveryInfo = dataTable.Rows[0]["Delivery Info"].ToString();
                            product.DeliveryInfoArabic = dataTable.Rows[0]["Delivery Info Arabic"].ToString();
                            product.BrandArabic = dataTable.Rows[0]["Brand Arabic"].ToString();
                            
                            product.CategoryArabic = dataTable.Rows[0]["Category Arabic"].ToString();
                            product.SubCategoryArabic = dataTable.Rows[0]["Sub Category Arabic"].ToString();
                            product.SubSubCategoryArabic = dataTable.Rows[0]["Sub Sub Category Arabic"].ToString();
                            product.ProductNameArabic = dataTable.Rows[0]["ProductName Arabic"].ToString();

                            product.CategoryImage = dataTable.Rows[0]["Category images"].ToString();

                            product.SubCategoryImage = dataTable.Rows[0]["Sub Category images"].ToString();

                            product.SubSubCategoryImage = dataTable.Rows[0]["Sub Sub Category images"].ToString();




                            #endregion


                            int counter = 0;
                            foreach (DataRow objDataRow in dataTable.Rows)
                            {
                                if (objDataRow.ItemArray.All(x => string.IsNullOrEmpty(x?.ToString())))
                                    continue;
                                counter++;
                            }

                            List<TempProduct> list = new List<TempProduct>();
                            List<TempProduct> Duplicatelist = new List<TempProduct>();
                            string productModel = string.Empty;

                            StringBuilder xmlStore = new StringBuilder();

                            using (XmlWriter writer = new XmlTextWriter(new StringWriter(xmlStore)))
                            {
                                writer.WriteStartElement("Root");

                                foreach (DataRow objDataRow in dataTable.Rows)
                                {
                                    if (objDataRow.ItemArray.All(x => string.IsNullOrEmpty(x?.ToString())))
                                        continue;
                                    TempProduct data = new TempProduct();                                  
                                    //product.SrNo = objDataRow["Sr.No"].ToString().Trim();
                                    product.Brand = objDataRow["Brand"].ToString().Trim();
                                    product.Barcode = objDataRow["Barcode"].ToString().Trim();
                                    product.Category = objDataRow["Category"].ToString().Trim();
                                    product.SubCategory = objDataRow["Sub Category"].ToString().Trim();
                                    product.SubSubCategory = objDataRow["Sub Sub Category"].ToString().Trim();
                                    product.ItemDesc = objDataRow["Item Desc"].ToString().Trim();
                                    product.AttributeCode = objDataRow["Attribute Code"].ToString().Trim();
                                    product.AttributeName = objDataRow["Attribute Name"].ToString().Trim();
                                    product.Images = objDataRow["Images"].ToString().Trim(); // Capitalized 'Images'
                                    product.Quantity = objDataRow["Quantitys"].ToString().Trim(); // Fixed column name from "Quantitys"
                                    product.SellingPrice = objDataRow["Selling Price"].ToString().Trim();
                                    product.ProductTags = objDataRow["Product Tags"].ToString().Trim();
                                    product.Stocks = objDataRow["Stocks Quantity"].ToString().Trim(); // Capitalized and fixed spacing
                                    product.ProductName = objDataRow["ProductName"].ToString().Trim();
                                    product.ProductType = objDataRow["Product Type"].ToString().Trim();
                                    product.AttributeNameArabic = objDataRow["Attribute Name Arabic"].ToString().Trim();
                                    product.DeliveryInfo = objDataRow["Delivery Info"].ToString().Trim();
                                    product.DeliveryInfoArabic = objDataRow["Delivery Info Arabic"].ToString().Trim();
                                    product.BrandArabic = objDataRow["Brand Arabic"].ToString().Trim();
                                    product.CategoryArabic = objDataRow["Category Arabic"].ToString().Trim();
                                    product.SubCategoryArabic = objDataRow["Sub Category Arabic"].ToString().Trim();
                                    product.SubSubCategoryArabic = objDataRow["Sub Sub Category Arabic"].ToString().Trim();
                                    product.ProductNameArabic = objDataRow["ProductName Arabic"].ToString().Trim();
                                    product.ItemDescArabic = objDataRow["Item Desc Arabic"].ToString().Trim();

                                    product.CategoryImage = objDataRow["Category images"].ToString().Trim();

                                    product.SubCategoryImage = objDataRow["Sub Category images"].ToString().Trim();

                                    product.SubSubCategoryImage = objDataRow["Sub Sub Category images"].ToString().Trim(); ;
                                    product.ExcelCount = counter;
                                    list.Add(data);

                                    // Xml File Create
                                    writer.WriteStartElement("Product");
                                    writer.WriteAttributeString("srNo", product.SrNo ?? " ");
                                    writer.WriteAttributeString("brand", product.Brand ?? " ");
                                    writer.WriteAttributeString("barcode", product.Barcode ?? " ");
                                    writer.WriteAttributeString("category", product.Category ?? " ");
                                    writer.WriteAttributeString("subCategory", product.SubCategory ?? " ");
                                    
                                    writer.WriteAttributeString("itemDesc", product.ItemDesc ?? " ");
                                    writer.WriteAttributeString("attributeCode", product.AttributeCode ?? " ");
                                    writer.WriteAttributeString("attributeName", product.AttributeName ?? " ");
                                    writer.WriteAttributeString("images", product.Images ?? " ");
                                    writer.WriteAttributeString("quantity", product.Quantity ?? " ");
                                    writer.WriteAttributeString("sellingPrice", product.SellingPrice ?? " ");
                                    writer.WriteAttributeString("productTags", product.ProductTags ?? " ");
                                    writer.WriteAttributeString("stocks", product.Stocks ?? " ");
                                    writer.WriteAttributeString("productName", product.ProductName ?? " ");
                                    writer.WriteAttributeString("subSubCategory", product.SubSubCategory ?? " ");
                                    writer.WriteAttributeString("productType", product.ProductType ?? " ");
                                    writer.WriteAttributeString("attributeNameArabic", product.AttributeNameArabic ?? " ");
                                    writer.WriteAttributeString("deliveryInfo", product.DeliveryInfo ?? " ");
                                    writer.WriteAttributeString("deliveryInfoArabic", product.DeliveryInfoArabic ?? " ");
                                    writer.WriteAttributeString("brandArabic", product.BrandArabic ?? " ");
                                    writer.WriteAttributeString("categoryArabic", product.CategoryArabic ?? " ");
                                    writer.WriteAttributeString("subCategoryArabic", product.SubCategoryArabic ?? " ");
                                    writer.WriteAttributeString("subSubCategoryArabic", product.SubSubCategoryArabic ?? " ");
                                    writer.WriteAttributeString("productNameArabic", product.ProductNameArabic ?? " ");
                                    writer.WriteAttributeString("itemDescArabic", product.ItemDescArabic ?? " ");
                                    writer.WriteAttributeString("categoryImage", product.CategoryImage ?? " ");
                                  
                                    writer.WriteAttributeString("subCategoryImage", product.SubCategoryImage ?? " ");
                                    writer.WriteAttributeString("subSubCategoryImage", product.SubSubCategoryImage ?? " ");
                                    writer.WriteAttributeString("excelCount", product.ExcelCount.ToString() ?? "0");
                                    writer.WriteEndElement();
                                }
                                writer.WriteEndElement();
                            }

                            if (list.Count() > 0)
                            {
                                await _tempMemberDataAccess.AddTempProduct(xmlStore.ToString(),feacherCategory.FeatureCategoryId);
                                return RedirectToAction(nameof(AddBulk), new { message = "", fCatagoryKey= feacherCategory.FeatureCategoryKey.ToString() });
                            }
                            else
                            {
                                return RedirectToAction(nameof(AddBulk), new { message = "List is Empty", fCatagoryKey = feacherCategory.FeatureCategoryKey.ToString() });
                            }


                        }
                        else
                        {
                            return RedirectToAction(nameof(AddBulk), new { message = "Invalid File Formate. Please Upload .xls or .xlsx format.", fCatagoryKey = feacherCategory.FeatureCategoryKey.ToString() });
                        }
                    }
                    else
                    {
                        return RedirectToAction(nameof(AddBulk), new { message = "No File Found.", fCatagoryKey = feacherCategory.FeatureCategoryKey.ToString() });
                    }
                }
                catch (Exception ex)
                {
                    return RedirectToAction(nameof(AddBulk), new { message = ex.Message.Replace("'", ""),fCatagoryKey = model.fCatagoryKey.ToString() });
                }
            }
            else
            {
                return RedirectToAction(nameof(AddBulk), new { message = "No File Found." , fCatagoryKey = model.fCatagoryKey.ToString() });
            }
        }


        public async Task<ActionResult> DeleteProduct(string fCatagoryKey)
        {
            _tempMemberDataAccess.DeleteTempProduct();
            return RedirectToAction("AddBulk", new { message = "",fCatagoryKey = fCatagoryKey });
        }

        public async Task<ActionResult> UpdateAllTemptoProduct(string fCatagoryKey)   
        {
            try
            {
                var feacherCategory = await _featureCategoryAccess.GetByKey(Guid.Parse(fCatagoryKey));
                _tempMemberDataAccess.AddProduct(feacherCategory.FeatureCategoryId);
                _tempMemberDataAccess.DeleteTempProduct();
                return RedirectToAction("AddBulk", new { message = "All Records Successfully Added to Member.", fCatagoryKey = fCatagoryKey });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<JsonResult>  ExcelImportCount()
        {
            var tempProduct = _tempMemberDataAccess.GetTempProductCount();
            var record = new
            {
                doneCount = tempProduct.DoneCount,
                totalCount = tempProduct.TotalCount,
                totalNew = tempProduct.TotalNew,
                totalDuplicate = tempProduct.TotalDuplicate
            };
            return Json(record, JsonRequestBehavior.AllowGet);
        }



        #endregion



        //[HttpPost]
        //public async Task<ActionResult> UploadExcel(HttpPostedFileBase excelFile, string fCatagoryKey)
        //{
        //    // Validate the feature category key
        //    if (string.IsNullOrEmpty(fCatagoryKey) || !Guid.TryParse(fCatagoryKey, out var featureCategoryKey))
        //    {
        //        TempData["ErrorMessage"] = "Invalid Feature Category Key.";
        //        TempData["AlertType"] = "danger";
        //        return RedirectToAction("Index");
        //    }

        //    // Call the service method
        //    var (insertedCount, failedCount, message, alertType) = await _productAccess.ProcessExcelProductUpload(excelFile, featureCategoryKey);

        //    // Store the result in TempData for display
        //    TempData["ErrorMessage"] = message;
        //    TempData["AlertType"] = alertType;

        //    // Redirect to the Index action, passing the feature category key
        //    return RedirectToAction("Index", new { fCatagoryKey = fCatagoryKey });
        //}
    }
}