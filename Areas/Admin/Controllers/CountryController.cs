using Boulevard.Models;
using Boulevard.Service;
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
    public class CountryController : Controller
    {
        public readonly CountryDataAccess _countryDataAccess;
        public CountryController()
        {
            _countryDataAccess = new CountryDataAccess();
        }
        // GET: Admin/Country
        public async Task<ActionResult> Index()
        {
            try
            {
                var Country = await _countryDataAccess.GetAll();
                if (Country.Count > 0)
                {
                    return View(Country);
                }
                else
                {
                    return View(new List<Country>());
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Country");
            }
        }


        [HttpGet]
        public async Task<ActionResult> CreateAndUpdate(string key)
        {
            try
            {
                Country data = new Country();
                if (string.IsNullOrEmpty(key))
                {
                    return View(data);
                }
                else
                {
                    data = await _countryDataAccess.GetCountryByKey(key);
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
        public async Task<ActionResult> CreateAndUpdate(Country model)
        {
            try
            {
                if (model != null)
                {
                    if (model.CountryId == 0)
                    {
                        model.CreateBy = 1;
                        model.CreateDate = Boulevard.Helper.DateTimeHelper.CreateDate();
                        await _countryDataAccess.Create(model);
                    }
                    else
                    {
                        model.UpdateBy = 1;
                        model.UpdateDate = Boulevard.Helper.DateTimeHelper.CreateDate();
                        await _countryDataAccess.Update(model);
                    }
                }

                return RedirectToAction("Index", "Country");
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
                Country modelData = await _countryDataAccess.GetCountryById(id);

                modelData.DeleteDate = Boulevard.Helper.DateTimeHelper.CreateDate();
                modelData.DeleteBy = id;
                modelData.Status = "Delete";
                await _countryDataAccess.Update(modelData);
                return RedirectToAction("Index", "Country");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}