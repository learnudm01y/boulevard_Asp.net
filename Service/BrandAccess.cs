using Boulevard.BaseRepository;
using Boulevard.Contexts;
using Boulevard.Helper;
using Boulevard.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;
using System.Xml.Linq;

namespace Boulevard.Service
{
    public class BrandAccess
    {
        public IUnitOfWork uow;
        public BrandAccess()
        {
            uow = new UnitOfWork();
        }

        /// <summary>
        /// Get All
        /// </summary>
        /// <returns></returns>
        public async Task<List<Brand>> GetAll()
        {
            try
            {
                return await uow.BrandRepository.Get().Where(e => e.Status.ToLower()!= "Deleted").Include(p=> p.FeatureCategory).OrderByDescending(s => s.BrandId).ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }
        
        public async Task<List<Brand>> GetAllByFeatureCategory(string fCatagoryKey)
        {
            try
            {
                var brand = new List<Brand>();
                var fCatagory = await uow.FeatureCategoryRepository.Get().FirstOrDefaultAsync(a => a.FeatureCategoryKey.ToString() == fCatagoryKey);
                if (fCatagory == null)
                {
                    brand = await uow.BrandRepository.Get().Where(e => e.Status.ToLower() != "Deleted").OrderBy(s => s.Title).ToListAsync();
                }
                else
                {
                    brand = await uow.BrandRepository.Get().Where(e => e.Status.ToLower() != "Deleted" && e.FeatureCategoryId == fCatagory.FeatureCategoryId).Include(p => p.FeatureCategory).OrderBy(s => s.Title).ToListAsync();
                }
                return brand;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Get By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Brand> GetById(int id)
        {
            try
            {
                return await uow.BrandRepository.GetById(id);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }
        public async Task<Brand> GetByBrandName(string brandName)
        {
            try
            {
                return await uow.BrandRepository.Get().Where(t => t.Title.ToLower().Trim()== brandName.Trim().ToLower()).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Get By key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<Brand> GetByKey(Guid key)
        {
            try
            {
                return await uow.BrandRepository.Get().Where(t => t.BrandKey == key).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Insert
        /// </summary>
        /// <returns></returns>
        public async Task Insert(Brand node)
        {
            try
            {
                node.BrandKey = Guid.NewGuid();
                node.CreateBy = 1;
                node.CreateDate = DateTimeHelper.DubaiTime();

                node.Status = "Active";
                await uow.BrandRepository.Add(node);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());

            }
        }
        public async Task<Brand> Insertv2(Brand node)
        {
            try
            {
                node.BrandKey = Guid.NewGuid();
                node.CreateBy = 1;
                node.CreateDate = DateTimeHelper.DubaiTime();

                node.Status = "Active";
               return await uow.BrandRepository.Add(node);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }
        /// <summary>
        /// Update
        /// </summary>
        /// <returns></returns>
        public async Task Update(Brand node)
        {
            try
            {
                var db = new BoulevardDbContext();
                node.UpdateBy = 1;
                node.UpdateDate = DateTimeHelper.DubaiTime();
                node.Status = "Active";
                db.Entry(node).State = EntityState.Modified;
                db.SaveChanges();
                //await uow.BrandRepository.Edit(node);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateby"></param>
        /// <returns></returns>
        public async Task<bool> Delete(Guid key, int updateby)
        {
            try
            {
                var exitResult = await GetByKey(key);
                if (exitResult != null)
                {
                    exitResult.DeleteBy = 1;
                    exitResult.DeleteDate = DateTimeHelper.DubaiTime();
                    exitResult.Status = "Deleted";
                    await uow.BrandRepository.Edit(exitResult);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// Upload Image
        /// </summary>
        /// <param name="flagImage"></param>
        /// <returns></returns>
        public string UploadImage(HttpPostedFileBase flagImage)
        {
            string ImageName = string.Empty;
            string Url = "/Content/Upload/Brand";
            ImageName = MediaHelper.UploadImage(flagImage, Url);
            return ImageName;
        }

        public string UploadMediumImage(HttpPostedFileBase mediumImage)
        {
            string ImageName = string.Empty;
            string Url = "/Content/Upload/Brand";
            ImageName = MediaHelper.UploadMediumFile(mediumImage, Url);
            return ImageName;
        }

        public string UploadLargeImage(HttpPostedFileBase largeImage)
        {
            string ImageName = string.Empty;
            string Url = "/Content/Upload/Brand";
            ImageName = MediaHelper.UploadLargeFile(largeImage, Url);
            return ImageName;
        }

        public async Task<List<Brand>> GetParentBrand(List<Brand> brandList)
        {
            try
            {
                brandList = brandList.Where(t => t.Status == "Active").ToList();
                return brandList;
            }
            catch (Exception ex)
            {
                return new List<Brand>();
            }
        }

    }
}