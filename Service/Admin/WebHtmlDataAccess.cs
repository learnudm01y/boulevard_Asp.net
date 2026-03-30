using Boulevard.BaseRepository;
using Boulevard.Contexts;
using Boulevard.Helper;
using Boulevard.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Boulevard.Service.Admin
{
    public class WebHtmlDataAccess
    {
        public IUnitOfWork uow;

        public WebHtmlDataAccess()
        {
            uow = new UnitOfWork();
        }

        /// <summary>
        /// Get All from WebHtml
        /// </summary>
        /// <returns></returns>
        public async Task<List<WebHtml>> GetAll()
        {
            try
            {
                return await uow.WebHtmlRepository.GetAll().Where(a => a.Status == "Active" && a.FeatureCategoryId==null).OrderByDescending(t => t.WebHtmlId).ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Get All WebHtml
        /// </summary>
        /// <param name="fCatagoryKey"></param>
        /// <returns></returns>
        public async Task<List<WebHtml>> GetAllWebHtml(string fCatagoryKey)
        {
            try
            {
                var fCatagory = await uow.FeatureCategoryRepository.Get().FirstOrDefaultAsync(a => a.FeatureCategoryKey.ToString() == fCatagoryKey);
                //var result = await uow.WebHtmlRepository.GetAll().Where(a => a.Status == "Active" && a.FeatureCategoryId == fCatagory.FeatureCategoryId).OrderByDescending(t => t.WebHtmlId).ToListAsync();
                if (fCatagory != null)
                {
                    return await uow.WebHtmlRepository.GetAll().Where(a => a.Status == "Active" && a.FeatureCategoryId == fCatagory.FeatureCategoryId).OrderByDescending(t => t.WebHtmlId).ToListAsync(); 
                }
                else
                {
                    return new List<WebHtml>();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Get WebHtml By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<WebHtml> GetWebHtmlById(long id)
        {
            try
            {
                return await uow.WebHtmlRepository.GetAll(a => a.WebHtmlId == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Get WebHtml By Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<WebHtml> GetWebHtmlByKey(string key)
        {
            try
            {
                return await uow.WebHtmlRepository.GetAll(a => a.WebHtmlkey.ToString() == key).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Create WebHtml
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<WebHtml> Create(WebHtml model)
        {
            try
            {
                model.WebHtmlkey = Guid.NewGuid();
                return await uow.WebHtmlRepository.Add(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Update WebHtml
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<WebHtml> Update(WebHtml model)
        {
            try
            {
                //var db = new BoulevardDbContext();
                //db.Entry(model).State = EntityState.Modified;
                //db.SaveChanges();
                return await uow.WebHtmlRepository.Edit(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        public string UploadPictureOne(HttpPostedFileBase PictureOne)
        {
            string ImageName = string.Empty;
            string Url = "/Content/Upload/WebHtml";
            ImageName = MediaHelper.UploadImage(PictureOne, Url);
            return ImageName;
        }

        /// <summary>
        /// UploadPictureTwo
        /// </summary>
        /// <param name="PictureTwo"></param>
        /// <returns></returns>
        public string UploadPictureTwo(HttpPostedFileBase PictureTwo)
        {
            string ImageName = string.Empty;
            string Url = "/Content/Upload/WebHtml";
            ImageName = MediaHelper.UploadImage(PictureTwo, Url);
            return ImageName;
        }

        /// <summary>
        /// UploadPictureThree
        /// </summary>
        /// <param name="PictureThree"></param>
        /// <returns></returns>
        public string UploadPictureThree(HttpPostedFileBase PictureThree)
        {
            string ImageName = string.Empty;
            string Url = "/Content/Upload/WebHtml";
            ImageName = MediaHelper.UploadImage(PictureThree, Url);
            return ImageName;
        }
    }
}