using Boulevard.Contexts;
using Boulevard.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Boulevard.Areas.Admin.Controllers
{
    public class DeliveryManagementController : Controller
    {
        // Product-based feature categories that support delivery via Jeeply
        private static readonly Dictionary<string, string> _productSections = new Dictionary<string, string>
        {
            { "3b317e3f-cb2f-4fdd-b9c8-3f2186695771", "Grocery" },
            { "88d5d23e-470f-409a-bb6b-def7ab1346fa", "Dessert & Flower" },
            { "E7B3A1C2-D4F5-4A6B-8C9D-1E2F3A4B5C6D", "Restaurant" },
            { "F1A2B3C4-D5E6-4F70-8B9C-0D1E2F3A4B5C", "Retail" },
        };

        // Ensure the IsDeliveryEnabled column exists (idempotent, runs once per app start)
        private static bool _columnEnsured = false;
        private static readonly object _columnLock = new object();

        private static void EnsureColumn(BoulevardDbContext db)
        {
            if (_columnEnsured) return;
            lock (_columnLock)
            {
                if (_columnEnsured) return;
                try
                {
                    db.Database.ExecuteSqlCommand(
                        @"IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                          WHERE TABLE_NAME='Brands' AND COLUMN_NAME='IsDeliveryEnabled')
                          ALTER TABLE Brands ADD IsDeliveryEnabled BIT NOT NULL DEFAULT 0");
                    _columnEnsured = true;
                }
                catch (Exception ex)
                {
                    Log.Error("EnsureColumn failed: " + ex);
                }
            }
        }

        public ActionResult Index()
        {
            ViewBag.Sections = _productSections;
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetMerchants(string fCategoryKey, string search = "", int page = 1, int pageSize = 20)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fCategoryKey) || !_productSections.ContainsKey(fCategoryKey))
                    return Json(new { success = false, message = "Invalid section." }, JsonRequestBehavior.AllowGet);

                using (var db = new BoulevardDbContext())
                {
                    EnsureColumn(db);

                    var featureCategory = await db.featureCategories
                        .FirstOrDefaultAsync(f => f.FeatureCategoryKey.ToString() == fCategoryKey);

                    if (featureCategory == null)
                        return Json(new { success = false, message = "Category not found." }, JsonRequestBehavior.AllowGet);

                    var query = db.Brands.Where(b => b.FeatureCategoryId == featureCategory.FeatureCategoryId
                                                  && b.Status != "Deleted");

                    if (!string.IsNullOrWhiteSpace(search))
                        query = query.Where(b => b.Title.Contains(search) || b.TitleAr.Contains(search));

                    int total = await query.CountAsync();

                    var brands = await query
                        .OrderBy(b => b.Title)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .Select(b => new
                        {
                            brandId = b.BrandId,
                            title = b.Title,
                            titleAr = b.TitleAr,
                            isDeliveryEnabled = b.IsDeliveryEnabled,
                            status = b.Status
                        })
                        .ToListAsync();

                    return Json(new { success = true, data = brands, total, page, pageSize }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return Json(new { success = false, message = "Server error: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> ToggleDelivery(int brandId)
        {
            try
            {
                using (var db = new BoulevardDbContext())
                {
                    EnsureColumn(db);

                    var brand = await db.Brands.FirstOrDefaultAsync(b => b.BrandId == brandId);
                    if (brand == null)
                        return Json(new { success = false, message = "Merchant not found." });

                    brand.IsDeliveryEnabled = !brand.IsDeliveryEnabled;
                    await db.SaveChangesAsync();

                    return Json(new { success = true, isDeliveryEnabled = brand.IsDeliveryEnabled });
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return Json(new { success = false, message = "Server error." });
            }
        }

        [HttpPost]
        public async Task<JsonResult> DeleteMerchant(int brandId)
        {
            try
            {
                using (var db = new BoulevardDbContext())
                {
                    var brand = await db.Brands.FirstOrDefaultAsync(b => b.BrandId == brandId && b.Status != "Deleted");
                    if (brand == null)
                        return Json(new { success = false, message = "Merchant not found." });

                    brand.Status = "Deleted";
                    await db.SaveChangesAsync();

                    Log.Information("Merchant deleted via DeliveryManagement: BrandId={BrandId} Title={Title}", brand.BrandId, brand.Title);
                    return Json(new { success = true });
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return Json(new { success = false, message = "Server error." });
            }
        }
    }
}

