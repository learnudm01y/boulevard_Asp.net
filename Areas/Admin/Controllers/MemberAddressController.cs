using Boulevard.Contexts;
using Boulevard.Models;
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
    public class MemberAddressController : Controller
    {
        private readonly MemberAddressDataAccess _memberAddressDataAccess;
        private readonly CountryDataAccess _countryDataAccess;
        private readonly CityDataAccess _cityDataAccess;
        public MemberAddressController()
        {
            _memberAddressDataAccess = new MemberAddressDataAccess();
            _countryDataAccess = new CountryDataAccess();
            _cityDataAccess = new CityDataAccess();
        } 
        // GET: Admin/MemberAddress
        public async Task<ActionResult> Index()
        {
            try
            {
                var data = await _memberAddressDataAccess.GetAll();
                if (data.Count > 0)
                {
                    return View(data);
                }
                else
                {
                    return View(new List<MemberAddress>());
                } 
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "MemberAddress");
            }
        }


        [HttpGet]
        public async Task<ActionResult> CreateAndUpdate(string key, long? memberId)
        {
            try
            {

                MemberAddress data = new MemberAddress();
                if(memberId != null)
                {
                    await LoadDdlV2();
                    ViewBag.MemberId = memberId;
                    return View(data);
                }
                else
                {
                    if (string.IsNullOrEmpty(key))
                    {
                        await LoadDdlV2();
                        ViewBag.MemberId = memberId;
                        return View(data);
                    }
                    else
                    {
                        await LoadDdl();
                        data = await _memberAddressDataAccess.GetMemberAddressByKey(key);
                        ViewBag.MemberId = data.MemberId;
                        return View(data);
                    }
                }
                
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateAndUpdate(MemberAddress model)
        {
            try
            {
                var db = new BoulevardDbContext();

                if (model != null)
                {
                    if (model.MemberAddressId == 0)
                    {
                        model.MemberAddressKey = Guid.NewGuid();
                        model.CreateBy = 1;
                        model.CreateDate = Boulevard.Helper.DateTimeHelper.CreateDate();
                        model.IsDefault = false;
                        await _memberAddressDataAccess.Create(model);
                    }
                    else
                    {
                        model.UpdateBy = 1;
                        model.UpdateDate = Boulevard.Helper.DateTimeHelper.CreateDate();
                        db.Entry(model).State = EntityState.Modified;
                        db.SaveChanges();
                        //await _memberDataAccess.Update(model);
                    }
                }
                var member = db.Members.FirstOrDefault(s => s.MemberId == model.MemberId);

                return RedirectToAction("Details", "Member", new { key = member.MemberKey });
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
                MemberAddress modelData = await _memberAddressDataAccess.GetMemberAddressById(id);

                modelData.DeleteDate = Boulevard.Helper.DateTimeHelper.CreateDate();
                modelData.DeleteBy = id;
                modelData.Status = "Delete";
                await _memberAddressDataAccess.Update(modelData);
                return RedirectToAction("Details", "Member", new { key = modelData.Member.MemberKey});
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


        //[HttpGet]
        //public JsonResult GetCityByCountryId(int countryId)
        //{
        //    var db = new BoulevardDbContext();

        //    var cities = db.Cities.Where(t => t.CountryId == countryId).ToList();
        //    var item = new SelectListItem
        //    {
        //        Value = "0",
        //        Selected = true,
        //        Text = "Select Country"
        //    };
        //    List<SelectListItem> selectCity = cities.Select(l => new SelectListItem
        //    {
        //        Value = l.CityId.ToString(),
        //        Text = l.CityName

        //    }).ToList();
        //    selectCity.Add(item);
        //    ViewBag.City = selectCity;
        //    return Json(cities);
        //}

        #region Drop Down

        private async Task LoadDdl()
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