using Boulevard.Areas.Admin.Data;
using Boulevard.Areas.Admin.Pagination;
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
    public class MemberController : BaseController
    {
        private readonly MemberDataAccess _memberDataAccess;
        private readonly MemberAddressDataAccess _memberAddressDataAccess;
        public MemberController()
        {
            _memberDataAccess = new MemberDataAccess();
            _memberAddressDataAccess = new MemberAddressDataAccess();
        }
        // GET: Admin/Member
        //public async Task<ActionResult> Index(int page = 1)
        //{
        //    try
        //    { var members = new List<Member>();
        //        //var data = await _memberDataAccess.GetAll();
        //        //if (data.Count > 0)
        //        //{
        //        //    return View(data);
        //        //}
        //        //else
        //        //{
        //        //    return View(new List<Member>());
        //        //}
        //        members = await _memberDataAccess.GetAll();
        //        return View(members.ToPagedList(page, 10));
        //    }
        //    catch (Exception ex)
        //    {
        //        return RedirectToAction("Index", "Member");
        //    }
        //}
        
        public async Task<ActionResult> Index(MemberViewModel memberViewModel)
        {
            try
            {
                var Members = new List<Member>();
                #region Page Size Cookies
                if (Request.Cookies["Page_Size"] != null)
                {
                    memberViewModel.PageSize = Convert.ToInt32(Request.Cookies["Page_Size"].Value);
                }
                else
                {
                    memberViewModel.PageSize = 10;
                    Response.Cookies["Page_Size"].Value = "10";
                    Response.Cookies["Page_Size"].Expires = DateTime.Now.AddDays(30);
                }
                #endregion


                var data = await _memberDataAccess.GetAllMember(memberViewModel);
                //var dd = data.Count();

                var memberOnPage = data;
                memberViewModel.Members = data.Members;
                //memberSearchResponse.TotalRecord = data.Members.Count();

                return View(memberViewModel);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Member");
            }
        }


        [HttpGet]
        public async Task<ActionResult> CreateAndUpdate(string key)
        {
            try
            {
                Member data = new Member();
                if (string.IsNullOrEmpty(key))
                {
                    return View(data);
                }
                else
                {
                    data = await _memberDataAccess.GetMemberByKey(key);
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
        public async Task<ActionResult> CreateAndUpdate(Member model, HttpPostedFileBase Image)
        {
            try
            {
                if (model != null)
                {
                    if (model.MemberId == 0)
                    {
                        if (Image != null)
                        {
                            model.Image = _memberDataAccess.UploadImage(Image);
                        }
                        model.CreateBy = 1;
                        model.CreateDate = Boulevard.Helper.DateTimeHelper.CreateDate();
                        await _memberDataAccess.Create(model);
                    }
                    else
                    {
                        var result = await _memberDataAccess.GetMemberById(model.MemberId);
                        if (Image != null)
                        {
                            model.Image = _memberDataAccess.UploadImage(Image);
                        }
                        else
                        {
                            model.Image = result.Image;
                        }
                        model.UpdateBy = 1;
                        model.UpdateDate = Boulevard.Helper.DateTimeHelper.CreateDate();
                        var db = new BoulevardDbContext();
                        db.Entry(model).State = EntityState.Modified;
                        db.SaveChanges();
                        //await _memberDataAccess.Update(model);
                    }
                }

                return RedirectToAction("Index", "Member");
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
                Member modelData = await _memberDataAccess.GetMemberById(id);

                modelData.DeleteDate = Boulevard.Helper.DateTimeHelper.CreateDate();
                modelData.DeleteBy = id;
                modelData.Status = "Delete";
                await _memberDataAccess.Update(modelData);
                return RedirectToAction("Index", "Member");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ActionResult> Details(string key)
        {
            try
            {

                var data = await _memberDataAccess.GetMemberByKey(key);
                ViewBag.MemberId = data.MemberId;
                if(data.MemberAddresses != null)
                {
                    data.MemberAddresses = await _memberAddressDataAccess.GetMemberAddressListByMemberId(data.MemberId);
                }
                return View(data);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }
    }
}