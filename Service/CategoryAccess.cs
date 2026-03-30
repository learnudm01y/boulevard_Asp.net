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
using System.Xml.Linq;

namespace Boulevard.Service
{
    public class CategoryAccess
    {
        public IUnitOfWork uow;
        public CategoryAccess()
        {
            uow = new UnitOfWork();
        }


        public async Task<List<Category>> GetParentChildWiseCategories(List<Category> categoryList)
        {
            try
            {
                foreach (var category in categoryList.Where(t => t.ParentId == 0))
                {
                    var childCategories = categoryList.Where(t => t.ParentId > 0 && t.ParentId == category.CategoryId).ToList();
                    if (childCategories.Any())
                    {
                        foreach (var child in childCategories)
                        {
                            var childCategorylist = categoryList.Where(t => (t.ParentId > 0 && t.ParentId == child.CategoryId));
                            if (childCategorylist.Any())
                            {
                                category.ChildCategories.Add(child);
                                category.ChildCategories[category.ChildCategories.Count - 1].ChildCategories.AddRange(childCategorylist);
                            }
                            else
                            {
                                category.ChildCategories.Add(child);
                            }
                        }
                    }
                }
                categoryList = categoryList.Where(t => t.ParentId == 0).ToList();
                return categoryList;
            }
            catch (Exception ex)
            {
                return new List<Category>();
            }
        }
        public async Task<List<Category>> GetParentChildWiseFCategories(List<Category> categoryList, string fCatagoryKey)
        {
            try
            {
                if (!string.IsNullOrEmpty(fCatagoryKey))
                {
                    var fCatagory = await uow.FeatureCategoryRepository.Get().Where(e => e.IsActive && e.FeatureCategoryKey.ToString() == fCatagoryKey).FirstOrDefaultAsync();
                    if (fCatagory != null)
                    {
                        foreach (var category in categoryList.Where(t => t.ParentId == 0 && t.FeatureCategoryId == fCatagory.FeatureCategoryId))
                        {
                            var childCategories = categoryList.Where(t => t.ParentId > 0 && t.ParentId == category.CategoryId).ToList();
                            if (childCategories.Any())
                            {
                                foreach (var child in childCategories)
                                {
                                    var childCategorylist = categoryList.Where(t => (t.ParentId > 0 && t.ParentId == child.CategoryId));
                                    if (childCategorylist.Any())
                                    {
                                        category.ChildCategories.Add(child);
                                        category.ChildCategories[category.ChildCategories.Count - 1].ChildCategories.AddRange(childCategorylist);
                                    }
                                    else
                                    {
                                        category.ChildCategories.Add(child);
                                    }
                                }
                            }
                        }
                    }
                }
                categoryList = categoryList.Where(t => t.ParentId == 0).ToList();
                return categoryList;
            }
            catch (Exception ex)
            {
                return new List<Category>();
            }
        }

        /// <summary>
        /// Get Parent Categories
        /// </summary>
        /// <param name="categoryList"></param>
        /// <returns></returns>
        public async Task<List<Category>> GetParentCategories(List<Category> categoryList)
        {
            try
            {
                categoryList = categoryList.Where(t => t.ParentId == 0).ToList();
                return categoryList;
            }
            catch (Exception ex)
            {
                return new List<Category>();
            }
        }


        /// <summary>
        /// Get All
        /// </summary>
        /// <returns></returns>
        public async Task<List<Category>> GetAll()
        {
            try
            {
                return await uow.CategoryRepository.Get().Where(e => e.Status.ToLower() == "active").ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }
        /// <summary>
        /// GetAllByFeatureCategory
        /// </summary>
        /// <param name="fCatagoryKey"></param>
        /// <returns></returns>
        public async Task<List<Category>> GetAllByFeatureCategory(string fCatagoryKey)
        {
            try
            {
                var catagory = new List<Category>();
                var fCatagory = await uow.FeatureCategoryRepository.Get().FirstOrDefaultAsync(a => a.FeatureCategoryKey.ToString() == fCatagoryKey);
                if (fCatagory == null)
                {
                    catagory = await uow.CategoryRepository.Get().Where(e => e.Status.ToLower() != "Deleted").ToListAsync();
                }
                else
                {
                    catagory = await uow.CategoryRepository.Get().Where(e => e.Status.ToLower() != "Deleted" && e.FeatureCategoryId == fCatagory.FeatureCategoryId).ToListAsync();
                }
                return catagory;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }


        /// <summary>
        /// GetAllCategoryByfCatagoryKey
        /// </summary>
        /// <param name="fCatagoryKey"></param>
        /// <returns></returns>
        public async Task<List<Category>> GetAllCategoryByfCatagoryKey(string fCatagoryKey)
        {
            try
            {
                var catagory = new List<Category>();
                var fCatagory = await uow.FeatureCategoryRepository.Get().FirstOrDefaultAsync(a => a.FeatureCategoryKey.ToString() == fCatagoryKey);
                if (fCatagory == null)
                {
                    catagory = await uow.CategoryRepository.Get().Where(e => e.Status.ToLower() != "Deleted" && e.ParentId == 0).ToListAsync();
                }
                else
                {
                    catagory = await uow.CategoryRepository.Get().Where(e => e.Status.ToLower() != "Deleted" && e.FeatureCategoryId == fCatagory.FeatureCategoryId && e.ParentId == 0).ToListAsync();
                }
                return catagory;
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
        public async Task<Category> GetById(int id)
        {
            try
            {
                return await uow.CategoryRepository.GetById(id);
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
        public async Task<Category> GetByKey(Guid key)
        {
            try
            {
                return await uow.CategoryRepository.Get().Where(t => t.CategoryKey == key).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }

        public async Task<Category> GetByCategoryName(string categoryName)
        {
            try
            {
                return await uow.CategoryRepository.Get()
                    .Where(t => t.CategoryName.ToLower().Trim() == categoryName.ToLower().Trim()&&t.ParentId==0)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }
        public async Task<Category> GetBySubCategoryName(string subCategoryName)
        {
            try
            {
                return await uow.CategoryRepository.Get()
                    .Where(t => t.CategoryName.ToLower().Trim() == subCategoryName.ToLower().Trim() && t.ParentId !=0)
                    .FirstOrDefaultAsync();
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
        public async Task Insert(Category node)
        {
            try
            {
                if (node.ParentId == null)
                {
                    node.ParentId = 0;
                }
                node.CategoryKey = Guid.NewGuid();
                node.CreateBy = 1;
                node.CreateDate = DateTimeHelper.DubaiTime();

                node.Status = "Active";
                await uow.CategoryRepository.Add(node);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        public async Task<Category> InsertV2(Category node)
        {
            try
            {
                if (node.ParentId == null)
                {
                    node.ParentId = 0;
                }
                node.CategoryKey = Guid.NewGuid();
                node.CreateBy = 1;
                node.CreateDate = DateTimeHelper.DubaiTime();

                node.Status = "Active";
               return await uow.CategoryRepository.Add(node);
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
        public async Task Update(Category node)
        {
            try
            {
                if (node.ParentId == null)
                {
                    node.ParentId = 0;
                }
                node.UpdateBy = 1;
                node.UpdateDate = DateTimeHelper.DubaiTime();
                node.Status = "Active";
                //await uow.CategoryRepository.Edit(node);
                var db = new BoulevardDbContext();
                db.Entry(node).State = EntityState.Modified;
                db.SaveChanges();
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
                    if (uow.CategoryRepository.Get().Any(e => e.ParentId == exitResult.CategoryId && e.Status.ToLower() == "active"))
                    {
                        return false;
                    }
                    else
                    {
                        exitResult.DeleteBy = 1;
                        exitResult.DeleteDate = DateTimeHelper.DubaiTime();
                        exitResult.Status = "Deleted";
                        await uow.CategoryRepository.Edit(exitResult);
                        return true;
                    }

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
            string Url = "/Content/Upload/Category";
            ImageName = MediaHelper.UploadImage(flagImage, Url);
            return ImageName;
        }

    }
}