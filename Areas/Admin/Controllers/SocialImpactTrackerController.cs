using Boulevard.Contexts;
using Boulevard.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Boulevard.Areas.Admin.Controllers
{
    public class SocialImpactTrackerController : BaseController
    {
        // GET: Admin/SocialImpactTracker
        public async Task<ActionResult> Index()
        {
            using (var db = new BoulevardDbContext())
            {
                var categories = await db.featureCategories
                    .Where(f => !f.IsDelete)
                    .OrderBy(f => f.Name)
                    .ToListAsync();

                // Remove duplicate category names in the UI.
                // Prefer entries that have a custom image when choosing which duplicate to keep.
                var distinct = categories
                    .GroupBy(f => (f.Name ?? string.Empty).Trim().ToLowerInvariant())
                    .Select(g => g
                        .OrderByDescending(x => !string.IsNullOrWhiteSpace(x.Image))
                        .ThenBy(x => x.FeatureCategoryId)
                        .First())
                    .ToList();

                return View(distinct);
            }
        }

        /// <summary>
        /// AJAX POST — update commission rate for a single FeatureCategory.
        /// Called by the inline edit in the table.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> UpdateCommission(int id, decimal rate)
        {
            if (rate < 0 || rate > 100)
                return Json(new { success = false, message = "Rate must be between 0 and 100." });

            using (var db = new BoulevardDbContext())
            {
                var cat = await db.featureCategories.FindAsync(id);
                if (cat == null)
                    return Json(new { success = false, message = "Category not found." });

                cat.CommissionRate = rate;
                await db.SaveChangesAsync();
                return Json(new { success = true, message = "Commission rate updated successfully.", rate = rate });
            }
        }
    }
}
