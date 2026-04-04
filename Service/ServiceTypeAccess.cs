using Boulevard.BaseRepository;
using Boulevard.Contexts;
using Boulevard.Helper;
using Boulevard.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Management;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace Boulevard.Service
{
    public class ServiceTypeAccess
    {
        public IUnitOfWork uow;
        public ServiceTypeAccess()
        {
            uow = new UnitOfWork();
        }

        /// <summary>
        /// Get All
        /// </summary>
        /// <returns></returns>
        public async Task<List<ServiceType>> GetAll(int featureCtegoryId = 0)
        {
            try
            {
                if (featureCtegoryId == 0)
                {
                    return await uow.ServiceTypesRepository.Get().Where(e => e.Status.ToLower() == "Active").Include(p => p.Service).ToListAsync();
                }
                else
                {
                    var services = await uow.ServiceRepository.Get().Where(a => a.Status == "Active" && a.FeatureCategoryId == featureCtegoryId).Select(a => a.ServiceId).ToListAsync();
                    if (services != null && services.Count() > 0)
                    {
                        return await uow.ServiceTypesRepository.Get().Where(e => e.Status.ToLower() == "Active" && services.Contains(e.ServiceId)).Include(p => p.Service).ToListAsync();
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }
        /// <summary>
        /// Get All ServiceType
        /// </summary>
        /// <param name="fCatagoryKey"></param>
        /// <returns></returns>
        public async Task<List<ServiceType>> GetAllServiceType(string fCatagoryKey)
        {
            try
            {
                var list = new List<ServiceType>();
                var serviceTypes = new ServiceType();
                var serviceIds = new List<int>();
                if (!string.IsNullOrEmpty(fCatagoryKey))
                {
                    var featureCategoryId = await uow.FeatureCategoryRepository.Get().Where(a => a.FeatureCategoryKey.ToString() == fCatagoryKey).Select(a => a.FeatureCategoryId).FirstOrDefaultAsync();
                    if (featureCategoryId > 0)
                    {
                        if (featureCategoryId == 9 || featureCategoryId == 11)
                        {
                            serviceIds = await uow.ServiceRepository.Get().Where(a => a.FeatureCategoryId == featureCategoryId && a.ParentId>0).Select(a => a.ServiceId).ToListAsync();
                        }
                        else
                        {
                            serviceIds = await uow.ServiceRepository.Get().Where(a => a.FeatureCategoryId == featureCategoryId).Select(a => a.ServiceId).ToListAsync();
                        }
                        
                        if (serviceIds != null)
                        {
                            foreach (var serviceId in serviceIds)
                            {
                                var serviceTypesData = await uow.ServiceTypesRepository.Get().Where(e => e.Status.ToLower() == "Active" && e.ServiceId == serviceId).Include(p => p.Service).ToListAsync();
                                if (serviceTypesData != null && serviceTypesData.Count() > 0)
                                {
                                    list.AddRange(serviceTypesData);
                                }
                            }
                        }
                    }
                }
                else
                {
                    list = await uow.ServiceTypesRepository.Get().Where(e => e.Status.ToLower() != "Delete").Include(p => p.Service).ToListAsync();
                }
                return list;
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
        public async Task<ServiceType> GetById(int id)
        {
            try
            {
                return await uow.ServiceTypesRepository.GetById(id);
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
        public async Task<ServiceType> GetByKey(Guid key)
        {
            try
            {
                ServiceType serviceType = await uow.ServiceTypesRepository.Get().Where(t => t.ServiceTypeKey == key).FirstOrDefaultAsync();
                List<int> categoryIds = await uow.ServiceCategoryRepository.Get().Where(t => t.ServiceTypeId == serviceType.ServiceTypeId).Select(p => p.CategoryId).ToListAsync();
                List<Category> Categories = await uow.CategoryRepository.Get().Where(p => categoryIds.Contains(p.CategoryId)).ToListAsync();
                serviceType.ServiceCategories = Categories;
                serviceType.ServiceTypeFiles = await uow.ServiceTypeFileRepository.Get().Where(p => p.ServiceTypeId == serviceType.ServiceTypeId && p.FileSource == "TypeImage").ToListAsync();

                return serviceType;
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
        public async Task<ServiceType> Insert(ServiceType node)
        {
            ServiceType insertData = null;
            try
            {
                node.ServiceTypeKey = Guid.NewGuid();
                node.CreateBy = 1;
                node.CreateDate = DateTimeHelper.DubaiTime();
                insertData = await uow.ServiceTypesRepository.Add(node);
                if (insertData != null)
                {
                    var service = await uow.ServiceRepository.GetById(insertData.ServiceId);
                    if (service != null)
                    {
                        var featureCategory = await uow.FeatureCategoryRepository.GetById(service.FeatureCategoryId);
                        if (featureCategory != null /*&& featureCategory.FeatureCategoryKey.ToString() == "dd501b2d-fe22-4c31-b340-1b4237fab5cc"*/)
                        {

                            if (!string.IsNullOrEmpty(node.SelectedCategoryId))
                            {
                                List<string> LeafCategories = node.SelectedCategoryId.Split(',').ToList();
                                foreach (string catId in LeafCategories)
                                {
                                    int ctgId = Convert.ToInt32(catId);
                                    ServiceCategory serviceCategory = new ServiceCategory()
                                    {
                                        CategoryId = Convert.ToInt32(ctgId),
                                        ServiceId = insertData.ServiceId,
                                        ServiceTypeId = insertData.ServiceTypeId,
                                        Status = "Active"
                                    };

                                    await uow.ServiceCategoryRepository.Add(serviceCategory);
                                    Category categoryNode = await uow.CategoryRepository.Get().Where(p => p.CategoryId == ctgId).FirstOrDefaultAsync();
                                    while (categoryNode.ParentId != null && categoryNode.ParentId > 0)
                                    {
                                        ServiceCategory serviceCat = new ServiceCategory()
                                        {
                                            CategoryId = categoryNode.ParentId.Value,
                                            ServiceId = insertData.ServiceId,
                                            ServiceTypeId = insertData.ServiceTypeId,
                                            Status = "Active"
                                        };
                                        await uow.ServiceCategoryRepository.Add(serviceCat);
                                        categoryNode = await uow.CategoryRepository.Get().Where(p => p.CategoryId == categoryNode.ParentId).FirstOrDefaultAsync();
                                    }
                                }
                            }

                        }
                    }
                }

                return insertData ?? node;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                // If the ServiceType row was committed before the exception, return it
                // so callers can still use the valid ServiceTypeId (e.g. for linked files).
                if (insertData != null && insertData.ServiceTypeId > 0)
                {
                    return insertData;
                }
                throw;
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <returns></returns>
        public async Task<ServiceType> Update(ServiceType node)
        {
            try
            {
                node.UpdateBy = 1;
                node.UpdateDate = DateTimeHelper.DubaiTime();
                var updateData = await uow.ServiceTypesRepository.Edit(node);


                List<ServiceCategory> serviceCat = await uow.ServiceCategoryRepository.Get().Where(p => p.ServiceTypeId == node.ServiceTypeId).ToListAsync();
                //List<ProductCategory> ProdCat = await uow.ProductCategoryRepository.Get().Where(p => p.ProductId == product.ProductId).ToListAsync();
                uow.ServiceCategoryRepository.MultipleRemove(serviceCat);

                if (updateData != null)
                {
                    var service = await uow.ServiceRepository.GetById(updateData.ServiceId);
                    if (service != null)
                    {
                        var featureCategory = await uow.FeatureCategoryRepository.GetById(service.FeatureCategoryId);
                        if (featureCategory != null)
                        {

                            if (!string.IsNullOrEmpty(node.SelectedCategoryId))
                            {
                                List<string> LeafCategories = node.SelectedCategoryId.Split(',').ToList();

                                foreach (string catId in LeafCategories)
                                {
                                    int ctgId = Convert.ToInt32(catId);
                                    if (await uow.ServiceCategoryRepository.Get().AnyAsync(s => s.ServiceTypeId == node.ServiceTypeId && s.CategoryId == ctgId) == false)
                                    {
                                        ServiceCategory serviceCategory = new ServiceCategory()
                                        {
                                            CategoryId = Convert.ToInt32(ctgId),
                                            ServiceId = updateData.ServiceId,
                                            ServiceTypeId = updateData.ServiceTypeId,
                                            Status = "Active"
                                        };

                                        await uow.ServiceCategoryRepository.Add(serviceCategory);
                                    }


                                    Category categoryNode = await uow.CategoryRepository.Get().Where(p => p.CategoryId == ctgId).FirstOrDefaultAsync();
                                    while (categoryNode.ParentId != null && categoryNode.ParentId > 0)
                                    {
                                        if (await uow.ServiceCategoryRepository.Get().AnyAsync(s => s.ServiceTypeId == node.ServiceTypeId && s.CategoryId == categoryNode.ParentId) == false)
                                        {
                                            ServiceCategory serCat = new ServiceCategory()
                                            {
                                                CategoryId = Convert.ToInt32(ctgId),
                                                ServiceId = updateData.ServiceId,
                                                ServiceTypeId = updateData.ServiceTypeId,
                                                Status = "Active"
                                            };

                                            await uow.ServiceCategoryRepository.Add(serCat);
                                        }
                                        categoryNode = await uow.CategoryRepository.Get().Where(p => p.CategoryId == categoryNode.ParentId).FirstOrDefaultAsync();
                                    }
                                }
                            }
                        }
                    }
                }

                return node;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return new ServiceType();
            }
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateby"></param>
        /// <returns></returns>
        public async Task<bool> Delete(Guid id, int updateby)
        {
            try
            {
                var exitResult = await GetByKey(id);
                if (exitResult != null)
                {
                    exitResult.DeleteBy = 1;
                    exitResult.DeleteDate = DateTimeHelper.DubaiTime();
                    exitResult.Status = "Deleted";
                    await uow.ServiceTypesRepository.Edit(exitResult);
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

        public async Task<bool> DeleteProductImage(int ImageId)
        {
            try
            {
                var exitResult = uow.ServiceTypeFileRepository.GetbyId(ImageId);
                if (exitResult != null)
                {
                    uow.ServiceTypeFileRepository.Remove(exitResult);
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
            string Url = "/Content/Upload/ServiceType";
            ImageName = MediaHelper.UploadImage(flagImage, Url);
            return ImageName;
        }

        public async Task<List<ServiceType>> GetAllServiceType(int fCategoryId = 13)
        {
            try
            {
                var featureCategory = await uow.FeatureCategoryRepository.GetById(fCategoryId);
                return await uow.ServiceTypesRepository.Get().Where(e => e.Status.ToLower() == "Active").Include(p => p.Service).ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }

    }
}