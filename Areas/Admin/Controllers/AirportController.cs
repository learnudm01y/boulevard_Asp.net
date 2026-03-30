using Boulevard.Contexts;
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
    public class AirportController : Controller
    {
        private readonly AirportDataAccess _airportDataAccess;
        private readonly CountryDataAccess _countryDataAccess;
        private readonly CityDataAccess _cityDataAccess;
        public AirportController()
        {
            _airportDataAccess = new AirportDataAccess();
            _countryDataAccess = new CountryDataAccess();
            _cityDataAccess = new CityDataAccess();
        }
        // GET: Admin/Airport
        public async Task<ActionResult> Index()
        {
            try
            {
                var airport = await _airportDataAccess.GetAll();
                if (airport.Count > 0)
                {
                    return View(airport);
                }
                else
                {
                    return View(new List<Airport>());
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Airport");
            }
        }

        [HttpGet]
        public async Task<ActionResult> CreateAndUpdate(string key)
        {
            try
            {
                Airport data = new Airport();
                if (string.IsNullOrEmpty(key))
                {
                    await LoadDdlV2();
                    return View(data);
                }
                else
                {
                    data = await _airportDataAccess.GetAirportByKey(key);
                    await LoadDdl(data.CountryId);
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
        public async Task<ActionResult> CreateAndUpdate(Airport model)
        {
            try
            {
                if (model != null)
                {
                    if (model.AirportId == 0)
                    {
                        model.CreateBy = 1;
                        model.CreateDate = Boulevard.Helper.DateTimeHelper.CreateDate();
                        await _airportDataAccess.Create(model);
                    }
                    else
                    {
                        await _airportDataAccess.Update(model);
                    }
                }

                return RedirectToAction("Index", "Airport");
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        public async Task<ActionResult> Delete(string key)
        {
            try
            {
                await _airportDataAccess.Delete(key);
                return RedirectToAction("Index", "MemberShip");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public JsonResult GetCityByCountryId(int countryId)
        {
            var db = new BoulevardDbContext();

            var cities = db.Cities.Where(t => t.CountryId == countryId).ToList();


            return this.Json(cities,
                       JsonRequestBehavior.AllowGet);
        }

        #region DropDown

        private async Task LoadDdl(int countryId)
        {
            var role = await _countryDataAccess.GetAll();
            var item = new SelectListItem
            {
                Value = "0",
                Selected = true,
                Text = "Select Country"
            };
            List<SelectListItem> selectCountry = role.Select(l => new SelectListItem
            {
                Value = l.CountryId.ToString(),
                Text = l.CountryName

            }).ToList();
            selectCountry.Add(item);
            ViewBag.Country = selectCountry;

            var city = await _cityDataAccess.GetCitiesByCountryId(countryId);
            var data = new SelectListItem
            {
                Value = "0",
                Selected = true,
                Text = ""
            };
            List<SelectListItem> selectCity = city.Select(l => new SelectListItem
            {
                Value = l.CityId.ToString(),
                Text = l.CityName

            }).ToList();
            selectCity.Add(data);
            ViewBag.City = selectCity;
        }


        private async Task LoadDdlV2()
        {
            var role = await _countryDataAccess.GetAll();
            var item = new SelectListItem
            {
                Value = "0",
                Selected = true,
                Text = "Select Country"
            };
            List<SelectListItem> selectCountry = role.Select(l => new SelectListItem
            {
                Value = l.CountryId.ToString(),
                Text = l.CountryName

            }).ToList();
            selectCountry.Add(item);
            ViewBag.Country = selectCountry;

            var city = await _cityDataAccess.GetAll();
            var data = new SelectListItem
            {
                Value = "0",
                Selected = true,
                Text = ""
            };
            List<SelectListItem> selectCity = new List<SelectListItem>();
            selectCity.Add(data);
            ViewBag.City = selectCity;
        }
        #endregion
    }
}