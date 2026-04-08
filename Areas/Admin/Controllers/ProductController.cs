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
                        productPrice = _context.ProductPrices
                            .Where(pp => pp.ProductId == p.ProductId && pp.Status == "Active")
                            .OrderBy(pp => pp.Price)
                            .Select(pp => (double?)pp.Price)
                            .FirstOrDefault() ?? (double)p.ProductPrice,
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
                if (!string.IsNullOrEmpty(model.FeatureCategoryKey))
                {
                    var featureCategory = await _featureCategoryAccess.GetByKey(Guid.Parse(model.FeatureCategoryKey));
                    if (featureCategory != null)
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
            var fCatagoryKeyFallback = model?.fCatagoryKey ?? "";
            if (Request.Files.Count > 0)
            {
                try
                {
                    _tempMemberDataAccess.DeleteTempProduct();

                    if (string.IsNullOrEmpty(model.fCatagoryKey) || !Guid.TryParse(model.fCatagoryKey, out Guid _fcGuid))
                        return RedirectToAction(nameof(AddBulk), new { message = "Invalid category key.", fCatagoryKey = model.fCatagoryKey });

                    var feacherCategory = await _featureCategoryAccess.GetByKey(Guid.Parse(model.fCatagoryKey));
                    if (feacherCategory == null)
                        return RedirectToAction(nameof(AddBulk), new { message = "Feature category not found in database. Please run the seed script first.", fCatagoryKey = model.fCatagoryKey });

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

                            // Read Excel using EPPlus — no OleDB/ACE driver required on server
                            var dataTable = new DataTable();
                            using (var epPkg = new ExcelPackage(new System.IO.FileInfo(path)))
                            {
                                var ws = epPkg.Workbook.Worksheets[1];
                                if (ws == null || ws.Dimension == null)
                                    return RedirectToAction(nameof(AddBulk), new { message = "Excel file is empty or could not be read.", fCatagoryKey = feacherCategory.FeatureCategoryKey.ToString() });

                                // Canonical column names — case-insensitive lookup so any variation
                                // in Excel header casing (e.g. "images" vs "Images") maps to exactly
                                // what the DataRow["ColumnName"] accesses below expect.
                                var canonicalCols = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                                {
                                    "Sr.No","Brand","Brand Arabic","Barcode",
                                    "Category","Category Arabic","Category images",
                                    "Sub Category","Sub Category Arabic","Sub Category images",
                                    "Sub Sub Category","Sub Sub Category Arabic","Sub Sub Category images",
                                    "MINI Category","MINI Category Arabic",
                                    "Item Desc","Item Desc Arabic",
                                    "Attribute Code","Attribute Name","Attribute Name Arabic",
                                    "Images","Quantitys","Selling Price","Product Tags","Stocks Quantity",
                                    "ProductName","ProductName Arabic","Product Type",
                                    "Delivery Info","Delivery Info Arabic",
                                    "ICV Boulevard Score","Origin"
                                };

                                // Build columns from header row
                                for (int c = 1; c <= ws.Dimension.Columns; c++)
                                {
                                    string rawName = (ws.Cells[1, c].Text ?? "").Trim();
                                    // Use canonical name if matched; preserve original otherwise
                                    string colName = canonicalCols.TryGetValue(rawName, out string canonical) ? canonical : rawName;
                                    string uniqueCol = colName;
                                    int suffix = 1;
                                    while (dataTable.Columns.Contains(uniqueCol))
                                        uniqueCol = colName + "_" + (++suffix);
                                    dataTable.Columns.Add(uniqueCol);
                                }

                                // Build data rows — read .Text to always get displayed value
                                for (int r = 2; r <= ws.Dimension.Rows; r++)
                                {
                                    var dr = dataTable.NewRow();
                                    for (int c = 1; c <= ws.Dimension.Columns; c++)
                                        dr[c - 1] = ws.Cells[r, c].Text ?? "";
                                    dataTable.Rows.Add(dr);
                                }
                            }

                            #region Check Excel Column – validate required headers exist before processing
                            // Access row[0] purely to confirm the columns exist; throws if missing.
                            var _chk = new
                            {
                                Brand              = dataTable.Rows[0]["Brand"].ToString(),
                                Barcode            = dataTable.Rows[0]["Barcode"].ToString(),
                                Category           = dataTable.Rows[0]["Category"].ToString(),
                                SubCategory        = dataTable.Rows[0]["Sub Category"].ToString(),
                                SubSubCategory     = dataTable.Rows[0]["Sub Sub Category"].ToString(),
                                ItemDesc           = dataTable.Rows[0]["Item Desc"].ToString(),
                                AttributeCode      = dataTable.Rows[0]["Attribute Code"].ToString(),
                                AttributeName      = dataTable.Rows[0]["Attribute Name"].ToString(),
                                Images             = dataTable.Rows[0]["Images"].ToString(),
                                Quantity           = dataTable.Rows[0]["Quantitys"].ToString(),
                                SellingPrice       = dataTable.Rows[0]["Selling Price"].ToString(),
                                ProductTags        = dataTable.Rows[0]["Product Tags"].ToString(),
                                Stocks             = dataTable.Rows[0]["Stocks Quantity"].ToString(),
                                ProductName        = dataTable.Rows[0]["ProductName"].ToString(),
                                ProductType        = dataTable.Rows[0]["Product Type"].ToString(),
                                AttrNameAr         = dataTable.Rows[0]["Attribute Name Arabic"].ToString(),
                                DeliveryInfo       = dataTable.Rows[0]["Delivery Info"].ToString(),
                                DeliveryInfoAr     = dataTable.Rows[0]["Delivery Info Arabic"].ToString(),
                                BrandAr            = dataTable.Rows[0]["Brand Arabic"].ToString(),
                                CategoryAr         = dataTable.Rows[0]["Category Arabic"].ToString(),
                                SubCategoryAr      = dataTable.Rows[0]["Sub Category Arabic"].ToString(),
                                SubSubCategoryAr   = dataTable.Rows[0]["Sub Sub Category Arabic"].ToString(),
                                ProductNameAr      = dataTable.Rows[0]["ProductName Arabic"].ToString(),
                                CategoryImage      = dataTable.Rows[0]["Category images"].ToString(),
                                SubCategoryImage   = dataTable.Rows[0]["Sub Category images"].ToString(),
                                SubSubCategoryImage = dataTable.Rows[0]["Sub Sub Category images"].ToString(),
                            };
                            #endregion


                            int counter = 0;
                            foreach (DataRow objDataRow in dataTable.Rows)
                            {
                                if (objDataRow.ItemArray.All(x => string.IsNullOrEmpty(x?.ToString())))
                                    continue;
                                counter++;
                            }

                            // Robust ICV column lookup — case-insensitive + trim whitespace from header
                            // (handles Excel files where the header cell has leading/trailing spaces)
                            string icvColName = null;
                            foreach (DataColumn dc in dataTable.Columns)
                            {
                                if (string.Equals(dc.ColumnName.Trim(), "ICV Boulevard Score", StringComparison.OrdinalIgnoreCase))
                                { icvColName = dc.ColumnName; break; }
                            }

                            // Origin column lookup — case-insensitive + trim whitespace from header
                            string originColName = null;
                            foreach (DataColumn dc in dataTable.Columns)
                            {
                                if (string.Equals(dc.ColumnName.Trim(), "Origin", StringComparison.OrdinalIgnoreCase))
                                { originColName = dc.ColumnName; break; }
                            }

                            int xmlRowCount = 0;

                            StringBuilder xmlStore = new StringBuilder();

                            using (XmlWriter writer = new XmlTextWriter(new StringWriter(xmlStore)))
                            {
                                writer.WriteStartElement("Root");

                                foreach (DataRow objDataRow in dataTable.Rows)
                                {
                                    if (objDataRow.ItemArray.All(x => string.IsNullOrEmpty(x?.ToString())))
                                        continue;

                                    // C2 fix: populate per-row TempProduct correctly.
                                    TempProduct data = new TempProduct();
                                    data.Brand               = objDataRow["Brand"].ToString().Trim();
                                    data.Barcode             = objDataRow["Barcode"].ToString().Trim();
                                    data.Category            = objDataRow["Category"].ToString().Trim();
                                    data.SubCategory         = objDataRow["Sub Category"].ToString().Trim();
                                    data.SubSubCategory      = objDataRow["Sub Sub Category"].ToString().Trim();
                                    data.ItemDesc            = objDataRow["Item Desc"].ToString().Trim();
                                    data.AttributeCode       = objDataRow["Attribute Code"].ToString().Trim();
                                    data.AttributeName       = objDataRow["Attribute Name"].ToString().Trim();
                                    data.Images              = objDataRow["Images"].ToString().Trim();
                                    data.Quantity            = objDataRow["Quantitys"].ToString().Trim();
                                    data.SellingPrice        = objDataRow["Selling Price"].ToString().Trim();
                                    data.ProductTags         = objDataRow["Product Tags"].ToString().Trim();
                                    data.Stocks              = objDataRow["Stocks Quantity"].ToString().Trim();
                                    data.ProductName         = objDataRow["ProductName"].ToString().Trim();
                                    // Product Type is handled safely below — see after SubSubCategoryImage block
                                    data.AttributeNameArabic = objDataRow["Attribute Name Arabic"].ToString().Trim();
                                    data.DeliveryInfo        = objDataRow["Delivery Info"].ToString().Trim();
                                    data.DeliveryInfoArabic  = objDataRow["Delivery Info Arabic"].ToString().Trim();
                                    data.BrandArabic         = objDataRow["Brand Arabic"].ToString().Trim();
                                    data.CategoryArabic      = objDataRow["Category Arabic"].ToString().Trim();
                                    data.SubCategoryArabic   = objDataRow["Sub Category Arabic"].ToString().Trim();
                                    data.SubSubCategoryArabic = objDataRow["Sub Sub Category Arabic"].ToString().Trim();
                                    data.ProductNameArabic   = objDataRow["ProductName Arabic"].ToString().Trim();
                                    data.ItemDescArabic      = objDataRow["Item Desc Arabic"].ToString().Trim();
                                    data.CategoryImage       = objDataRow["Category images"].ToString().Trim();
                                    data.SubCategoryImage    = objDataRow["Sub Category images"].ToString().Trim();
                                    data.SubSubCategoryImage = objDataRow["Sub Sub Category images"].ToString().Trim();
                                    // MINI Category (4th level). Commas inside category name cells are
                                    // part of the name — they are NEVER split as value separators here.
                                    data.MiniCategory        = dataTable.Columns.Contains("MINI Category")
                                                               ? objDataRow["MINI Category"].ToString().Trim() : "";
                                    data.MiniCategoryArabic  = dataTable.Columns.Contains("MINI Category Arabic")
                                                               ? objDataRow["MINI Category Arabic"].ToString().Trim() : "";
                                    // Product Type is optional in the template; default to "Now" when absent.
                                    data.ProductType         = dataTable.Columns.Contains("Product Type")
                                                               ? objDataRow["Product Type"].ToString().Trim() : "Now";
                                    // ICV Boulevard Score is optional — column AE in the Excel template.
                                    // Column name matching is case-insensitive and whitespace-tolerant (icvColName resolved above).
                                    data.IcvBoulevardScore   = icvColName != null
                                                               ? (objDataRow[icvColName] ?? "").ToString().Trim() : "";
                                    // Origin is optional — e.g. "Local", "Imported".
                                    data.Origin              = originColName != null
                                                               ? (objDataRow[originColName] ?? "").ToString().Trim() : "";
                                    data.ExcelCount          = counter;
                                    xmlRowCount++;

                                    // Xml File Create — use 'data' (the correctly-populated per-row object)
                                    writer.WriteStartElement("Product");
                                    writer.WriteAttributeString("srNo", data.SrNo ?? " ");
                                    writer.WriteAttributeString("brand", data.Brand ?? " ");
                                    writer.WriteAttributeString("barcode", data.Barcode ?? " ");
                                    writer.WriteAttributeString("category", data.Category ?? " ");
                                    writer.WriteAttributeString("subCategory", data.SubCategory ?? " ");
                                    writer.WriteAttributeString("subSubCategory", data.SubSubCategory ?? " ");
                                    writer.WriteAttributeString("itemDesc", data.ItemDesc ?? " ");
                                    writer.WriteAttributeString("attributeCode", data.AttributeCode ?? " ");
                                    writer.WriteAttributeString("attributeName", data.AttributeName ?? " ");
                                    writer.WriteAttributeString("images", data.Images ?? " ");
                                    writer.WriteAttributeString("quantity", data.Quantity ?? " ");
                                    writer.WriteAttributeString("sellingPrice", data.SellingPrice ?? " ");
                                    writer.WriteAttributeString("productTags", data.ProductTags ?? " ");
                                    writer.WriteAttributeString("stocks", data.Stocks ?? " ");
                                    writer.WriteAttributeString("productName", data.ProductName ?? " ");
                                    writer.WriteAttributeString("productType", data.ProductType ?? " ");
                                    writer.WriteAttributeString("attributeNameArabic", data.AttributeNameArabic ?? " ");
                                    writer.WriteAttributeString("deliveryInfo", data.DeliveryInfo ?? " ");
                                    writer.WriteAttributeString("deliveryInfoArabic", data.DeliveryInfoArabic ?? " ");
                                    writer.WriteAttributeString("brandArabic", data.BrandArabic ?? " ");
                                    writer.WriteAttributeString("categoryArabic", data.CategoryArabic ?? " ");
                                    writer.WriteAttributeString("subCategoryArabic", data.SubCategoryArabic ?? " ");
                                    writer.WriteAttributeString("subSubCategoryArabic", data.SubSubCategoryArabic ?? " ");
                                    writer.WriteAttributeString("productNameArabic", data.ProductNameArabic ?? " ");
                                    writer.WriteAttributeString("itemDescArabic", data.ItemDescArabic ?? " ");
                                    writer.WriteAttributeString("categoryImage", data.CategoryImage ?? " ");
                                    writer.WriteAttributeString("subCategoryImage", data.SubCategoryImage ?? " ");
                                    writer.WriteAttributeString("subSubCategoryImage", data.SubSubCategoryImage ?? " ");
                                    writer.WriteAttributeString("miniCategory", data.MiniCategory ?? " ");
                                    writer.WriteAttributeString("miniCategoryArabic", data.MiniCategoryArabic ?? " ");
                                    writer.WriteAttributeString("icvBoulevardScore", data.IcvBoulevardScore ?? "");
                                    writer.WriteAttributeString("origin", data.Origin ?? "");
                                    writer.WriteAttributeString("excelCount", data.ExcelCount.ToString());
                                    writer.WriteEndElement();
                                }
                                writer.WriteEndElement();
                            }

                            if (xmlRowCount > 0)
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
                    return RedirectToAction(nameof(AddBulk), new { message = ex.Message.Replace("'", ""), fCatagoryKey = fCatagoryKeyFallback });
                }
            }
            else
            {
                return RedirectToAction(nameof(AddBulk), new { message = "No File Found.", fCatagoryKey = fCatagoryKeyFallback });
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
                if (string.IsNullOrEmpty(fCatagoryKey) || !Guid.TryParse(fCatagoryKey, out Guid fcGuid))
                    return RedirectToAction("AddBulk", new { message = "Invalid category key.", fCatagoryKey = fCatagoryKey });

                var feacherCategory = await _featureCategoryAccess.GetByKey(fcGuid);
                if (feacherCategory == null)
                    return RedirectToAction("AddBulk", new { message = "Feature category not found.", fCatagoryKey = fCatagoryKey });

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