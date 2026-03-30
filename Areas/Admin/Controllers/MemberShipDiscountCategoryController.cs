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
    public class MemberShipDiscountCategoryController : Controller
    {
        private readonly MemberShipDiscountCategoryDataAccess _memberShipDiscountCategoryDataAccess;
        private readonly FeatureCategoryAccess _featureCategoryAccess;
        private readonly MemberShipDataAccess _memberShipDataAccess;
        public MemberShipDiscountCategoryController()
        {
            _memberShipDiscountCategoryDataAccess = new MemberShipDiscountCategoryDataAccess();
            _featureCategoryAccess = new FeatureCategoryAccess();
            _memberShipDataAccess = new MemberShipDataAccess();
        }
        // GET: Admin/MemberShipDiscountCategory
        public async Task<ActionResult> Index()
        {
            try
            {
                var memberShip = await _memberShipDiscountCategoryDataAccess.GetAll();
                if (memberShip.Count > 0)
                {
                    return View(memberShip);
                }
                else
                {
                    return View(new List<MemberShipDiscountCategory>());
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "MemberShipDiscountCategory");
            }
        }

        [HttpGet]
        public async Task<ActionResult> CreateAndUpdate(int? modelId)
        {
            try
            {
                var featureCategories = new SelectList(await _featureCategoryAccess.GetAll(), "FeatureCategoryId", "Name");
                if (featureCategories != null)
                {
                    ViewBag.featureCategory = featureCategories;
                }
                var memberShipDropDown = new SelectList(await _memberShipDataAccess.GetAll(), "MemberShipId", "Title");
                if (memberShipDropDown != null)
                {
                    ViewBag.Membership = memberShipDropDown;
                }
                MemberShipDiscountCategory data = new MemberShipDiscountCategory();
                if (modelId == null)
                {
                    return View(data);
                }
                else
                {
                    data = await _memberShipDiscountCategoryDataAccess.GetMemberShipDiscountCategoryById(modelId.Value);
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
        public async Task<ActionResult> CreateAndUpdate(MemberShipDiscountCategory model)
        {
            try
            {
                if (model != null)
                {
                    if (model.MemberShipDiscountCategoryId == 0)
                    {
                        var membershipDiscount = await _memberShipDiscountCategoryDataAccess.GetMemberShipDiscountCategoryByMemberShipId(model.MemberShipId, model.FeatureCategoryId);
                        if (membershipDiscount == null)
                        {
                            await _memberShipDiscountCategoryDataAccess.Create(model);
                        }
                        else
                        {
                            TempData["Message"] = "MemberShip discount category is already exist!";
                            return RedirectToAction("CreateAndUpdate", "MemberShipDiscountCategory");
                        }
                    }
                    else
                    {
                        var membershipDiscountOld = await _memberShipDiscountCategoryDataAccess.GetMemberShipDiscountCategoryById(model.MemberShipDiscountCategoryId);
                        if (membershipDiscountOld != null)
                        {
                            var membershipDiscount = await _memberShipDiscountCategoryDataAccess.GetMemberShipDiscountCategoryByMemberShipId(membershipDiscountOld.MemberShipId, membershipDiscountOld.FeatureCategoryId);
                            if (membershipDiscount != null)
                            {
                                await _memberShipDiscountCategoryDataAccess.Update(model);
                            }
                            //else
                            //{
                            //    TempData["Message"] = "MemberShip discount category is already exist!";
                            //    return RedirectToAction("CreateAndUpdate", "MemberShipDiscountCategory", new { modelId  = membershipDiscountOld .MemberShipDiscountCategoryId});
                            //}
                        }
                        //await _memberShipDiscountCategoryDataAccess.Update(model);
                    }
                }

                return RedirectToAction("Index", "MemberShipDiscountCategory");
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }
    }
}