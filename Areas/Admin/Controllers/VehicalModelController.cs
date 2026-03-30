using Boulevard.Contexts;
using Boulevard.Models;
using Boulevard.Service;
using Boulevard.Service.Admin;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Boulevard.Areas.Admin.Controllers
{
    public class VehicalModelController : BaseController
    {
        private readonly VehicalModelDataAccess _vehicalModelDataAccess;
        private readonly BrandAccess _brandAccess;
        public VehicalModelController()
        {
            _vehicalModelDataAccess = new VehicalModelDataAccess();
            _brandAccess = new BrandAccess();
        }
        // GET: Admin/VehicalModel
        public async Task<ActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> CreateAndUpdate(string key)
        {
            try
            {
                await LoadDdl();
                VehicalModel data = new VehicalModel();
                if (string.IsNullOrEmpty(key))
                {
                    return View(data);
                }
                else
                {
                    data = await _vehicalModelDataAccess.GetVehicalModelByKey(key);
                    return View(data);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateAndUpdate(VehicalModel model)
        {
            try
            {
                if (model != null)
                {
                    var user = GetUser();
                    if (model.VehicalModelId == 0)
                    {
                        model.CreateBy = user.UserId;
                        model.CreateDate = Boulevard.Helper.DateTimeHelper.CreateDate();
                        await _vehicalModelDataAccess.Create(model);
                    }
                    else
                    {
                        model.UpdateBy = user.UserId;
                        model.UpdateDate = Boulevard.Helper.DateTimeHelper.CreateDate();
                        await _vehicalModelDataAccess.Update(model);
                    }
                }

                return RedirectToAction("Index", "VehicalModel");
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }


        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                VehicalModel modelData = await _vehicalModelDataAccess.GetVehicalModelById(id);

                modelData.DeleteDate = Boulevard.Helper.DateTimeHelper.CreateDate();
                modelData.DeleteBy = GetUser().UserId;
                modelData.Status = "Delete";
                await _vehicalModelDataAccess.Update(modelData);
                return RedirectToAction("Index", "VehicalModel");
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region Drop Down

        [HttpGet]
        public async Task<ActionResult> GetPagedVehicalModels(string searchTerm, int page = 1, int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 10) pageSize = 10;
                if (pageSize > 500) pageSize = 500;
                var db = new BoulevardDbContext();
                var query = db.VehicalModels.Where(v => v.Status != "Delete");
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var term = searchTerm.Trim().ToLower();
                    query = query.Where(v => v.VehicalModelName.ToLower().Contains(term));
                }
                var rows = await query
                    .OrderByDescending(v => v.VehicalModelId)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(v => new {
                        vehicalModelId = v.VehicalModelId,
                        vehicalModelKey = v.VehicalModelKey,
                        vehicalModelName = v.VehicalModelName,
                        brandTitle = v.Brand.Title,
                        modelDetails = v.ModelDetails,
                        status = v.Status
                    })
                    .ToListAsync();
                return Json(new { success = true, rows }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetPagedVehicalModelsCount(string searchTerm)
        {
            try
            {
                var db = new BoulevardDbContext();
                var query = db.VehicalModels.Where(v => v.Status != "Delete");
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var term = searchTerm.Trim().ToLower();
                    query = query.Where(v => v.VehicalModelName.ToLower().Contains(term));
                }
                int total = await query.CountAsync();
                return Json(new { success = true, total }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Drop Down

        private async Task LoadDdl()
        {
            var db = new BoulevardDbContext();
            var brand = await db.Brands.Where(a => a.Status == "Active" && a.FeatureCategoryId == 3).ToListAsync();
            //var brand = await _brandAccess.GetAll();
            var item = new SelectListItem
            {
                Value = "0",
                Selected = true,
                Text = "Select Brand"
            };
            List<SelectListItem> selectBrand = brand.Select(l => new SelectListItem
            {
                Value = l.BrandId.ToString(),
                Text = l.Title

            }).ToList();
            ViewBag.Brand = selectBrand;
        }

        #endregion
    }
}