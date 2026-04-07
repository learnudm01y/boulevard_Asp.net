using Boulevard.Contexts;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Boulevard.Controllers
{
    /// <summary>
    /// Social Impact Tracker API — exposes commission rates per FeatureCategory for mobile consumption.
    /// Base URL: /api/v1/social-impact
    /// </summary>
    public class SocialImpactController : BaseController
    {
        /// <summary>
        /// GET /api/v1/social-impact/commissions
        /// Returns all (non-deleted) feature categories with their current commission rate.
        /// </summary>
        [HttpGet]
        public async Task<IHttpActionResult> GetCommissions()
        {
            try
            {
                using (var db = new BoulevardDbContext())
                {
                    string baseUrl = System.Web.HttpContext.Current.Request.Url
                        .GetLeftPart(UriPartial.Authority) + "/";

                    var cats = await db.featureCategories
                        .Where(f => !f.IsDelete)
                        .OrderBy(f => f.Name)
                        .ToListAsync();

                    var result = cats.Select(f => new
                    {
                        id             = f.FeatureCategoryId,
                        key            = f.FeatureCategoryKey,
                        name           = f.Name,
                        nameAr         = f.NameAr,
                        featureType    = f.FeatureType,
                        isActive       = f.IsActive,
                        image          = string.IsNullOrEmpty(f.Image) ? null : baseUrl + f.Image,
                        commissionRate = f.CommissionRate ?? 0m,
                        commissionLabel= $"{f.CommissionRate ?? 0m:0.##}%"
                    }).ToList();

                    return SuccessMessage(result);
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new
                {
                    isSuccess = false,
                    message   = "Internal server error",
                    code      = HttpStatusCode.InternalServerError,
                    result    = new List<object>()
                });
            }
        }

        /// <summary>
        /// GET /api/v1/social-impact/commissions/{key}
        /// Returns the commission rate for a single category by its FeatureCategoryKey (GUID).
        /// </summary>
        [HttpGet]
        public async Task<IHttpActionResult> GetCommissionByKey(Guid key)
        {
            try
            {
                using (var db = new BoulevardDbContext())
                {
                    string baseUrl = System.Web.HttpContext.Current.Request.Url
                        .GetLeftPart(UriPartial.Authority) + "/";

                    var f = await db.featureCategories
                        .FirstOrDefaultAsync(c => c.FeatureCategoryKey == key && !c.IsDelete);

                    if (f == null)
                        return ErrorMessage("Category not found.");

                    var result = new
                    {
                        id             = f.FeatureCategoryId,
                        key            = f.FeatureCategoryKey,
                        name           = f.Name,
                        nameAr         = f.NameAr,
                        featureType    = f.FeatureType,
                        isActive       = f.IsActive,
                        image          = string.IsNullOrEmpty(f.Image) ? null : baseUrl + f.Image,
                        commissionRate = f.CommissionRate ?? 0m,
                        commissionLabel= $"{f.CommissionRate ?? 0m:0.##}%"
                    };

                    return SuccessMessage(result);
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new
                {
                    isSuccess = false,
                    message   = "Internal server error",
                    code      = HttpStatusCode.InternalServerError,
                    result    = new List<object>()
                });
            }
        }
    }
}
