using Boulevard.Models;
using Boulevard.Service.Admin;
using Boulevard.Service.WebAPI;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Boulevard.Areas.Admin.Controllers
{
    public class MemberShipController : Controller
    {
        private readonly MemberShipDataAccess _memberShipDataAccess;
        public MemberShipController()
        {
            _memberShipDataAccess = new MemberShipDataAccess();
        }
        // GET: Admin/MemberShip
        public async Task<ActionResult> Index()
        {
            try
            {
                var memberShip = await _memberShipDataAccess.GetAll();
                if (memberShip.Count > 0)
                {
                    return View(memberShip);
                }
                else
                {
                    return View(new List<MemberShip>());
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "MemberShip");
            }
        }

        [HttpGet]
        public async Task<ActionResult> CreateAndUpdate(string key)
        {
            try
            {
                MemberShip data = new MemberShip();
                if (string.IsNullOrEmpty(key))
                {
                    return View(data);
                }
                else
                {
                    data = await _memberShipDataAccess.GetMemberShipByKey(key);
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
        public async Task<ActionResult> CreateAndUpdate(MemberShip model, HttpPostedFileBase images, HttpPostedFileBase membershipBannerAr)
        {
            try
            {
                if (model != null)
                {
                    if (model.MemberShipId == 0)
                    {
                        if (images != null)
                        {
                            model.MembershipBanner = _memberShipDataAccess.UploadImage(images);
                        }
                        if (membershipBannerAr != null)
                        {
                            model.MembershipBannerAr = _memberShipDataAccess.UploadMembershipBannerAr(membershipBannerAr);
                        }
                        model.CreateBy = 1;
                        model.CreateDate = Boulevard.Helper.DateTimeHelper.CreateDate();
                        await _memberShipDataAccess.Create(model);
                    }
                    else
                    {
                        var modelData = await _memberShipDataAccess.GetMemberShipById(model.MemberShipId);

                        if (images != null)
                        {
                            model.MembershipBanner = _memberShipDataAccess.UploadImage(images);
                        }
                        else
                        {
                            if (modelData.MembershipBanner != null)
                            {
                                model.MembershipBanner = modelData.MembershipBanner;
                            }
                        }
                        
                        if (membershipBannerAr != null)
                        {
                            model.MembershipBannerAr = _memberShipDataAccess.UploadMembershipBannerAr(membershipBannerAr);
                        }
                        else
                        {
                            if (modelData.MembershipBannerAr != null)
                            {
                                model.MembershipBannerAr = modelData.MembershipBannerAr;
                            }
                        }
                        await _memberShipDataAccess.Update(model);
                    }
                }

                return RedirectToAction("Index", "MemberShip");
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
                await _memberShipDataAccess.Delete(key);
                return RedirectToAction("Index", "MemberShip");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}