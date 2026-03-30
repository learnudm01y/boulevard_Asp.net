using Boulevard.Models;
using Boulevard.Service.Admin;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Boulevard.Areas.Admin.Controllers
{
    public class CityController : Controller
    {
        private readonly CityDataAccess _cityDataAccess;
        private readonly CountryDataAccess _countryDataAccess;
        public CityController()
        {
            _cityDataAccess = new CityDataAccess();
            _countryDataAccess = new CountryDataAccess();
        }

        // GET: Admin/City
        public async Task<ActionResult> Index()
        {
            try
            {
                var data = await _cityDataAccess.GetAll();
                if (data.Count > 0)
                {
                    return View(data);
                }
                else
                {
                    return View(new List<City>());
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }


        [HttpGet]
        public async Task<ActionResult> CreateAndUpdate(string key)
        {
            try
            {
                await LoadDdl();
                City data = new City();
                if (string.IsNullOrEmpty(key))
                {
                    return View(data);
                }
                else
                {
                    data = await _cityDataAccess.GetCityByKey(key);
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
        public async Task<ActionResult> CreateAndUpdate(City model)
        {
            try
            {
                await LoadDdl();
                if (model != null)
                {
                    if (model.CityId == 0)
                    {
                        model.CreateBy = 1;
                        model.CreateDate = Boulevard.Helper.DateTimeHelper.CreateDate();
                        await _cityDataAccess.Create(model);
                    }
                    else
                    {
                        model.UpdateBy = 1;
                        model.UpdateDate = Boulevard.Helper.DateTimeHelper.CreateDate();
                        await _cityDataAccess.Update(model);
                    }
                }

                return RedirectToAction("Index", "City");
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
                City modelData = await _cityDataAccess.GetCityById(id);
                modelData.DeleteDate = Boulevard.Helper.DateTimeHelper.CreateDate();
                modelData.DeleteBy = id;
                modelData.Status = "Deleted";
                await _cityDataAccess.Update(modelData);
                return RedirectToAction("Index", "City");
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region Drop Down

        private async Task LoadDdl()
        {
            var role = await _countryDataAccess.GetAll();
            List<SelectListItem> selectRole = role.Select(l => new SelectListItem
            {
                Value = l.CountryId.ToString(),
                Text = l.CountryName

            }).ToList();
            ViewBag.Country = selectRole;
        }

        #endregion
    }
}