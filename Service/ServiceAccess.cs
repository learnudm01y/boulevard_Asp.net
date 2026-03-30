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
using System.Web.Services.Description;
using System.Xml.Linq;

namespace Boulevard.Service
{
    public class ServiceAccess
    {
        public IUnitOfWork uow;
        public ServiceAccess()
        {
            uow = new UnitOfWork();
        }

        /// <summary>
        /// Get All
        /// </summary>
        /// <returns></returns>
        public async Task<List<Boulevard.Models.Service>> GetAll()
        {
            try
            {
                return await uow.ServiceRepository.Get().Where(e => e.Status.ToLower() != "Delete" && e.IsPackage == false).Include(p => p.FeatureCategory).ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }
        /// <summary>
        /// Get All By FeatureCategory
        /// </summary>
        /// <param name="fCatagoryKey"></param>
        /// <returns></returns>
        public async Task<List<Boulevard.Models.Service>> GetAllByFeatureCategory(string fCatagoryKey)
        {
            try
            {
                var result = new List<Boulevard.Models.Service>();
                if (!string.IsNullOrEmpty(fCatagoryKey))
                {
                    var fCatagory = await uow.FeatureCategoryRepository.Get().FirstOrDefaultAsync(a => a.FeatureCategoryKey.ToString() == fCatagoryKey);
                    result = await uow.ServiceRepository.Get().Where(e => e.Status.ToLower() == "Active" && e.FeatureCategoryId == fCatagory.FeatureCategoryId && e.IsPackage==false && e.ParentId==0).Include(p => p.FeatureCategory).ToListAsync();
                }
                else
                {
                    result = await uow.ServiceRepository.Get().Where(e => e.Status.ToLower() == "Active" && e.IsPackage == false && e.ParentId == 0).Include(p => p.FeatureCategory).ToListAsync();
                }
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }


        public async Task<List<Boulevard.Models.Service>> GetAllByFeatureCategoryForChildService(string fCatagoryKey)
        {
            try
            {
                var result = new List<Boulevard.Models.Service>();
                if (!string.IsNullOrEmpty(fCatagoryKey))
                {
                    var fCatagory = await uow.FeatureCategoryRepository.Get().FirstOrDefaultAsync(a => a.FeatureCategoryKey.ToString() == fCatagoryKey);
                    result = await uow.ServiceRepository.Get().Where(e => e.Status.ToLower() == "Active" && e.FeatureCategoryId == fCatagory.FeatureCategoryId && e.IsPackage == false && e.ParentId > 0).Include(p => p.FeatureCategory).ToListAsync();
                }
                else
                {
                    result = await uow.ServiceRepository.Get().Where(e => e.Status.ToLower() == "Active" && e.IsPackage == false && e.ParentId >0).Include(p => p.FeatureCategory).ToListAsync();
                }
                if (result != null && result.Count() > 0)
                {
                    foreach (var res in result)
                    {
                        if (res.ParentId > 0)
                        {
                            res.ParentServiceName = await uow.ServiceRepository.Get().Where(s => s.ServiceId == res.ParentId).Select(s => s.Name).FirstOrDefaultAsync();
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }

        public async Task<List<Boulevard.Models.Service>> GetServicesByFeatureCategoryForPackage(string fCatagoryKey)
        {
            try
            {
                var result = new List<Boulevard.Models.Service>();
                if (!string.IsNullOrEmpty(fCatagoryKey))
                {
                    var fCatagory = await uow.FeatureCategoryRepository.Get().FirstOrDefaultAsync(a => a.FeatureCategoryKey.ToString() == fCatagoryKey);
                    result = await uow.ServiceRepository.Get().Where(e => e.Status.ToLower() == "Active" && e.FeatureCategoryId == fCatagory.FeatureCategoryId && e.IsPackage == true).Include(p => p.FeatureCategory).ToListAsync();
                }
                else
                {
                    result = await uow.ServiceRepository.Get().Where(e => e.Status.ToLower() == "Active" && e.IsPackage == false).Include(p => p.FeatureCategory).ToListAsync();
                }
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }

        public async Task<List<Boulevard.Models.ServiceType>> GetServicesTypeByFeatureCategoryForPackage(string fCatagoryKey)
        {
            try
            {
                var resulttype = new List<Boulevard.Models.ServiceType>();
                var result = new List<Boulevard.Models.Service>();
                if (!string.IsNullOrEmpty(fCatagoryKey))
                {
                    var fCatagory = await uow.FeatureCategoryRepository.Get().FirstOrDefaultAsync(a => a.FeatureCategoryKey.ToString() == fCatagoryKey);
                    result = await uow.ServiceRepository.Get().Where(e => e.Status.ToLower() == "Active" && e.FeatureCategoryId == fCatagory.FeatureCategoryId).Include(p => p.FeatureCategory).ToListAsync();
                    if (result != null && result.Count() > 0)
                    {
                        foreach (var res in result)
                        {
                            var types = await uow.ServiceTypesRepository.Get().Where(e => e.Status.ToLower() == "Active" && e.ServiceId == res.ServiceId).ToListAsync();
                            if (types != null && types.Count() > 0)
                            {
                                resulttype.AddRange(types);
                            }
                        }
                    }

                }
                else
                {
                    result = await uow.ServiceRepository.Get().Where(e => e.Status.ToLower() == "Active").Include(p => p.FeatureCategory).ToListAsync();
                    if (result != null && result.Count() > 0)
                    {
                        foreach (var res in result)
                        {
                            var types = await uow.ServiceTypesRepository.Get().Where(e => e.Status.ToLower() == "Active" && e.ServiceId == res.ServiceId).ToListAsync();
                            if (types != null && types.Count() > 0)
                            {
                                resulttype.AddRange(types);
                            }
                        }
                    }
                }
                return resulttype;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Get All
        /// </summary>
        /// <returns></returns>
        public async Task<List<Boulevard.Models.Service>> GetAllMotorServices()
        {
            try
            {
                List<Boulevard.Models.Service> list = new List<Models.Service>();
                Boulevard.Models.Service obj1 = new Models.Service()
                {
                    ServiceId = 1,
                    Name = "Orgenja Bikers"
                };
                Boulevard.Models.Service obj2 = new Models.Service()
                {
                    ServiceId = 2,
                    Name = "Bicycle Fix"
                };
                Boulevard.Models.Service obj3 = new Models.Service()
                {
                    ServiceId = 3,
                    Name = "Doctor Bikes"
                };
                Boulevard.Models.Service obj4 = new Models.Service()
                {
                    ServiceId = 4,
                    Name = "Cosmo Bikers"
                };
                Boulevard.Models.Service obj5 = new Models.Service()
                {
                    ServiceId = 5,
                    Name = "Cleaver Fixer"
                };
                list.Add(obj1);
                list.Add(obj2);
                list.Add(obj3);
                list.Add(obj4);
                list.Add(obj5);
                return list;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }
        public async Task<List<Boulevard.Models.Service>> GetAllSalonServices()
        {
            try
            {
                List<Boulevard.Models.Service> list = new List<Models.Service>();
                Boulevard.Models.Service obj1 = new Models.Service()
                {
                    ServiceId = 1,
                    Name = "Opra Salon"
                };
                Boulevard.Models.Service obj2 = new Models.Service()
                {
                    ServiceId = 2,
                    Name = "Quick Buck Salon"
                };
                Boulevard.Models.Service obj3 = new Models.Service()
                {
                    ServiceId = 3,
                    Name = "AK Salon"
                };
                Boulevard.Models.Service obj4 = new Models.Service()
                {
                    ServiceId = 4,
                    Name = "Grom From"
                };
                Boulevard.Models.Service obj5 = new Models.Service()
                {
                    ServiceId = 5,
                    Name = "AliBaBa Salon"
                };
                list.Add(obj1);
                list.Add(obj2);
                list.Add(obj3);
                list.Add(obj4);
                list.Add(obj5);
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
        public async Task<Boulevard.Models.Service> GetById(int id)
        {
            try
            {
                return await uow.ServiceRepository.GetById(id);
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
        public async Task<Boulevard.Models.Service> GetByKey(Guid key)
        {
            try
            {
                Boulevard.Models.Service node = await uow.ServiceRepository.Get().Where(t => t.ServiceKey == key).FirstOrDefaultAsync();
                node.ServiceImages = await uow.ServiceImageRepository.Get().Where(p => p.ServiceId == node.ServiceId).ToListAsync();
                return node;
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
        public async Task<Boulevard.Models.Service> Insert(Boulevard.Models.Service node)
        {
            try
            {
                node.ServiceKey = Guid.NewGuid();
                node.CreateDate = DateTimeHelper.DubaiTime();
                Boulevard.Models.Service model = await uow.ServiceRepository.Add(node);
                foreach (var item in node.ServiceImageURLs)
                {
                    ServiceImage img = new ServiceImage()
                    {
                        ServiceId = model.ServiceId,
                        Image = item,
                        IsFeature = true,
                        CreateBy = node.CreateBy,
                        CreateDate = DateTimeHelper.DubaiTime()
                    }; ;
                    await uow.ServiceImageRepository.Add(img);
                }
                return model;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return new Boulevard.Models.Service();
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <returns></returns>
        public async Task<Boulevard.Models.Service> Update(Boulevard.Models.Service node)
        {
            try
            {
                node.UpdateDate = DateTimeHelper.DubaiTime();
                var db = new BoulevardDbContext();
                db.Entry(node).State = EntityState.Modified;
                db.SaveChanges();
                //Boulevard.Models.Service model = await uow.ServiceRepository.Edit(node);
                //Boulevard.Models.Service model = await uow.ServiceRepository.Edit(node);
                foreach (var item in node.ServiceImageURLs)
                {
                    ServiceImage img = new ServiceImage()
                    {
                        ServiceId = node.ServiceId,
                        Image = item,
                        IsFeature = true,
                        CreateBy = node.UpdateBy.Value,
                        CreateDate = DateTimeHelper.DubaiTime()
                    };
                    await uow.ServiceImageRepository.Add(img);
                }
                return node;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return new Boulevard.Models.Service();
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
                    await uow.ServiceRepository.Edit(exitResult);
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
            string Url = "/Content/Upload/Service";
            ImageName = MediaHelper.UploadImage(flagImage, Url);
            return ImageName;
        }
        /// <summary>
        /// Delete Image
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateby"></param>
        /// <returns></returns>
        public async Task<bool> DeleteProductImage(int ImageId)
        {
            try
            {
                var exitResult = uow.ServiceImageRepository.GetbyId(ImageId);
                if (exitResult != null)
                {
                    uow.ServiceImageRepository.Remove(exitResult);
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

        #region Package

        /// <summary>
        /// GetbAllbPackage By FeatureCategory
        /// </summary>
        /// <param name="fCatagoryKey"></param>
        /// <returns></returns>
        public async Task<List<Boulevard.Models.Service>> GetAllPackageByFeatureCategory(string fCatagoryKey)
        {
            try
            {
                var result = new List<Boulevard.Models.Service>();
                if (!string.IsNullOrEmpty(fCatagoryKey))
                {
                    var fCatagory = await uow.FeatureCategoryRepository.Get().FirstOrDefaultAsync(a => a.FeatureCategoryKey.ToString() == fCatagoryKey);
                    result = await uow.ServiceRepository.Get().Where(e => e.Status.ToLower() == "Active" && e.IsPackage == true && e.FeatureCategoryId == fCatagory.FeatureCategoryId).Include(p => p.FeatureCategory).ToListAsync();
                }
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }
        /// <summary>
        /// Insert Service For Package
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public async Task<Boulevard.Models.Service> InsertForPackage(Boulevard.Models.Service node)
        {
            try
            {
                node.ServiceKey = Guid.NewGuid();
                node.CreateDate = DateTimeHelper.DubaiTime();
                node.IsPackage = true;
                Boulevard.Models.Service model = await uow.ServiceRepository.Add(node);
                foreach (var item in node.ServiceImageURLs)
                {
                    ServiceImage img = new ServiceImage()
                    {
                        ServiceId = model.ServiceId,
                        Image = item,
                        IsFeature = true,
                        CreateBy = node.CreateBy,
                        CreateDate = DateTimeHelper.DubaiTime()
                    }; ;
                    await uow.ServiceImageRepository.Add(img);
                }
                return model;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return new Boulevard.Models.Service();
            }
        }
        /// <summary>
        /// Update Service For Package
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public async Task<Boulevard.Models.Service> UpdateForPackage(Boulevard.Models.Service node)
        {
            try
            {
                node.UpdateDate = DateTimeHelper.DubaiTime();
                node.IsPackage = true;
                var db = new BoulevardDbContext();
                db.Entry(node).State = EntityState.Modified;
                db.SaveChanges();
                //Boulevard.Models.Service model = await uow.ServiceRepository.Edit(node);
                //Boulevard.Models.Service model = await uow.ServiceRepository.Edit(node);
                foreach (var item in node.ServiceImageURLs)
                {
                    ServiceImage img = new ServiceImage()
                    {
                        ServiceId = node.ServiceId,
                        Image = item,
                        IsFeature = true,
                        CreateBy = node.UpdateBy.Value,
                        CreateDate = DateTimeHelper.DubaiTime()
                    };
                    await uow.ServiceImageRepository.Add(img);
                }
                return node;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return new Boulevard.Models.Service();
            }
        }
        #endregion
    }
}