using Boulevard.BaseRepository;
using Boulevard.Models;
using Boulevard.ResponseModel;
using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;
using System.Web.Services.Description;

namespace Boulevard.Service.WebAPI
{
    public class CategoryServiceAccess
    {
        public IUnitOfWork uow;
        public CategoryServiceAccess()
        {
            uow = new UnitOfWork();
        }

        string link = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/";
        public async Task<Category> Get(int categoryId,string lang="en")
        {
            try
            {



                if (categoryId != 0)
                {
                    var category = await uow.CategoryRepository.Get().Where(s => s.CategoryId == categoryId && s.Status == "Active").FirstOrDefaultAsync();

                    if (category == null)
                    {
                        return null;
                    }
                    else
                    {
                        category.CategoryName = lang == "en" ? category.CategoryName : category.CategoryNameAr;
                        category.CategoryDescription = lang == "en" ? category.CategoryDescription : category.CategoryDescriptionAr;
                        if (!string.IsNullOrEmpty(category.Image))
                        {
                            category.Image = link + category.Image;
                        }

                        if (!string.IsNullOrEmpty(category.Icon))
                        {
                            category.Icon = link + category.Icon;
                        }
                        return category;
                    }
                }
                else
                {
                    return null;
                }

                   
            }
            catch (Exception ex)
            {

                return null;
                
            }
        }

        public async Task<List<Category>> GetAll(int featureCategoryId=0, string type = "All", string lang = "en")
        {

            try
            {
                var categories = new List<Category>();
                if (featureCategoryId == 0)
                {
                    if (type == "All")
                    {
                        categories = await uow.CategoryRepository.Get().Where(s => s.Status == "Active").OrderBy(s => s.label).ToListAsync();
                    }
                    else
                    {
                        categories = await uow.CategoryRepository.Get().Where(s => s.Status == "Active").OrderByDescending(s => s.CategoryId).Take(5).ToListAsync();
                    }
                }
                else
                {
                    if (type == "All")
                    {
                        categories = await uow.CategoryRepository.Get().Where(e => e.FeatureCategoryId == featureCategoryId && e.Status == "Active").OrderBy(s => s.label).ToListAsync();
                    }
                    else
                    {
                       
                        categories = await uow.CategoryRepository.Get().Where(e => e.FeatureCategoryId == featureCategoryId && e.Status == "Active").OrderBy(s => s.CategoryId).Take(5).ToListAsync();
                    }
                }
                if (categories != null && categories.Count > 0)
                {
                    foreach (var category in categories)
                    {
                        category.CategoryName = lang=="en"?category.CategoryName : category.CategoryNameAr;
                        category.CategoryDescription = lang == "en" ? category.CategoryDescription : category.CategoryDescriptionAr;
                        if (!string.IsNullOrEmpty(category.Image))
                        {
                            category.Image = link + category.Image;
                        }

                        if (!string.IsNullOrEmpty(category.Icon))
                        {
                            category.Icon = link + category.Icon;
                        }
                    }
                    return categories;
                }
                else
                {
                    return null;
                }

                
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public async Task<List<Category>> GetAllParent(int featureCategoryId = 0,int size=10,int count=0,int memberId=0,string lang="en")
        {

            try
            {
                if (count > 0)
                {
                    count = count * size;
                }
                var categories = new List<Category>();
                if (featureCategoryId == 0)
                {
                    categories = await uow.CategoryRepository.Get().Where(s => s.Status == "Active" && s.ParentId==0).ToListAsync();
                }
                else
                {
                    categories = await uow.CategoryRepository.Get().Where(e => e.FeatureCategoryId == featureCategoryId && e.Status == "Active" && e.ParentId == 0).ToListAsync();
                }
                if (categories != null && categories.Count > 0)
                {
                   
                    foreach (var category in categories)
                    {
                        category.CategoryName = lang == "en" ? category.CategoryName : category.CategoryNameAr;
                        category.CategoryDescription = lang == "en" ? category.CategoryDescription : category.CategoryDescriptionAr;

                        if (!string.IsNullOrEmpty(category.Image))
                        {
                            category.Image = link + category.Image;
                        }

                        var productids = await uow.ProductCategoryRepository.Get().Where(s => s.CategoryId == category.CategoryId && s.Status=="Active").OrderByDescending(s=>s.ProductId).Skip(count).Take(size).Select(s => s.ProductId).ToListAsync();

                        if (productids.Count() > 0|| productids!=null)
                        {
                            foreach (var productid in productids)
                            {
                                var productResult = await new ProductServiceAccess().getSmallDetailsProducts(productid,memberId,false,lang);
                                if (productResult != null)
                                {
                                    category.Products.Add(productResult);
                                }
                            }
                        }

                    }
                    return categories;
                }
                else
                {
                    return null;
                }


            }
            catch (Exception ex)
            {

                return null;
            }
        }


      


        public async Task<Category> GetCategorywiseProduct(int categoryId, string keyword = "", int size = 10, int count = 0,int brandId=0,int memberId = 0, bool IsScheduled = false,string lang="en",int productType=1)
        {

            try
            {
                var proType = new List<int>() { productType, 3 };
                if (count > 0)
                {
                    count = count * size;
                }
                var productids = new List<int>();

                var  categories = await uow.CategoryRepository.Get().Where(s => s.Status == "Active" && s.CategoryId == categoryId ).FirstOrDefaultAsync();
                
               
                if (categories != null )
                {
                    categories.CategoryName = lang == "en" ? categories.CategoryName : categories.CategoryNameAr;
                    categories.CategoryDescription = lang == "en" ? categories.CategoryDescription : categories.CategoryDescriptionAr;

                    if (!string.IsNullOrEmpty(categories.Image))
                        {
                        categories.Image = link + categories.Image;
                        }

                    if (!string.IsNullOrEmpty(keyword))
                    {
                        var searchWords = keyword.ToLower().Split(" ".ToCharArray(),
                            StringSplitOptions.RemoveEmptyEntries);

                        productids = await uow.ProductCategoryRepository.Get().Where(s => s.CategoryId == categories.CategoryId && s.Status == "Active").OrderByDescending(s => s.ProductId).Select(s => s.ProductId).Skip(count).Take(size).ToListAsync();
                        if (productids.Count() > 0 && productids != null)
                        {
                            productids = await uow.ProductRepository.Get().Where(s => searchWords.Any(t => s.ProductName.ToLower().Contains(t)) && productids.Contains(s.ProductId) && s.Status == "Active" && proType.Contains(s.ProductType)).OrderByDescending(s => s.ProductId).Select(s => s.ProductId).ToListAsync();
                        }

                        //                    var searchWords = (keyword ?? "")
                        //.Trim()
                        //.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                        //.Select(x => x.ToLower())
                        //.ToList();

                        //                    // Step 1: Get product IDs under this category
                        //                    var categoryProductIds = await uow.ProductCategoryRepository.Get()
                        //                        .Where(s => s.CategoryId == categories.CategoryId && s.Status == "Active")
                        //                        .Select(s => s.ProductId)
                        //                        .ToListAsync();

                        //                    if (categoryProductIds != null && categoryProductIds.Any())
                        //                    {
                        //                        // Step 2: Base product query
                        //                        var query = uow.ProductRepository.Get()
                        //                            .Where(s => s.Status == "Active" &&
                        //                                        (s.ProductType == productType || s.ProductType == 3) &&
                        //                                        categoryProductIds.Contains(s.ProductId));

                        //                        // Step 3: Apply search if keyword provided
                        //                        if (searchWords.Any())
                        //                        {
                        //                            // Build OR conditions manually (EF6-friendly)
                        //                            foreach (var term in searchWords)
                        //                            {
                        //                                string t = term;
                        //                                query = query.Where(s => s.ProductName.ToLower().Contains(t));
                        //                            }
                        //                        }

                        //                        // Step 4: Apply ordering and paging
                        //                        productids = await query
                        //                            .OrderByDescending(s => s.ProductId)
                        //                            .Skip(count)
                        //                            .Take(size)
                        //                            .Select(s => s.ProductId)
                        //                            .ToListAsync();
                        //                    }
                    }
                    else
                    {
                        if (brandId == 0)
                        {
                            productids = await uow.ProductCategoryRepository.Get().Where(s => s.CategoryId == categories.CategoryId && s.Status == "Active").OrderByDescending(s => s.ProductId).Select(s => s.ProductId).Skip(count).Take(size).ToListAsync();
                        }
                        else
                        {
                            var ids  = await uow.ProductCategoryRepository.Get().Where(s => s.CategoryId == categories.CategoryId && s.Status == "Active").OrderByDescending(s => s.ProductId).Select(s => s.ProductId).ToListAsync();
                            if (ids.Count() > 0 || ids != null)
                            {
                                productids = await uow.ProductRepository.Get().Where(s=>s.BrandId==brandId && ids.Contains(s.ProductId) && s.Status == "Active" &&  proType.Contains(s.ProductType)).OrderByDescending(s => s.ProductId).Select(s => s.ProductId).Skip(count).Take(size).ToListAsync();
                            }
                        }
                    }

                        if (productids.Count() > 0 || productids != null)
                        {
                            foreach (var productid in productids)
                            {
                                var productResult = await new ProductServiceAccess().getSmallDetailsProducts(productid, memberId,false,lang);
                                if (productResult != null)
                                {
                                categories.Products.Add(productResult);
                                }
                            }
                        }

                    
                    return categories;
                }
                else
                {
                    return null;
                }


            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public async Task<Category> GetCategorywiseService(int categoryId, string keyword = "", int size = 0, int count = 0,int memberId=0,string lang="en")
        {

            try
            {
                if (count > 0)
                {
                    count = count * size;
                }
                var serviceTypeids = new List<int>();

                var categories = await uow.CategoryRepository.Get().Where(s => s.Status == "Active" && s.CategoryId == categoryId).FirstOrDefaultAsync();


                if (categories != null)
                {
                    categories.CategoryName = lang == "en" ? categories.CategoryName : categories.CategoryNameAr;
                    categories.CategoryDescription = lang == "en" ? categories.CategoryDescription : categories.CategoryDescriptionAr;

                    if (!string.IsNullOrEmpty(categories.Image))
                    {
                        categories.Image = link + categories.Image;
                    }

                    if (!string.IsNullOrEmpty(categories.Icon))
                    {
                        categories.Image = link + categories.Icon;
                    }

                    if (!string.IsNullOrEmpty(keyword))
                    {
                        var searchWords = keyword.ToLower().Split(" ".ToCharArray(),
                            StringSplitOptions.RemoveEmptyEntries);

                       var ids = await uow.ServiceCategoryRepository.Get().Where(s => s.CategoryId == categories.CategoryId && s.Status == "Active").OrderByDescending(s => s.ServiceTypeId).Select(s => s.ServiceTypeId).Skip(count).Take(size).ToListAsync();
                        if (ids.Count() > 0 && ids != null)
                        {
                            serviceTypeids = await uow.ServiceTypesRepository.Get().Where(s => searchWords.Any(t => s.ServiceTypeName.ToLower().Contains(t)) && ids.Contains(s.ServiceTypeId) && s.Status == "Active").OrderByDescending(s => s.ServiceTypeId).Select(s => s.ServiceTypeId).ToListAsync();
                           
                        }
                    }
                    else
                    {
                        serviceTypeids = await uow.ServiceCategoryRepository.Get()
                      .Include(s => s.ServiceType)
                      .Where(s =>
                          s.CategoryId == categories.CategoryId &&
                          s.Status == "Active" &&
                          s.ServiceType.Status == "Active"
                      )
                      .OrderByDescending(s => s.ServiceTypeId)
                      .Select(s => s.ServiceTypeId.Value)
                      .Skip(count)
                      .Take(size)
                      .ToListAsync();

                    }

                    if (serviceTypeids.Count() > 0 || serviceTypeids != null)
                    {
                        var services = await uow.ServiceTypesRepository.Get().Where(s => serviceTypeids.Contains(s.ServiceTypeId )  && s.Status == "Active").Select(s=>s.ServiceId).Distinct().ToListAsync();

                        if (services.Count() == 1)
                        {
                            var ServiceResult = await new ServiceAccess().GetSmallServices(services[0], 0, memberId, lang,serviceTypeids);
                            if (ServiceResult != null)
                            {
                                categories.Services.Add(ServiceResult);
                            }
                        }
                        else
                        {
                            foreach (var serviceTypeId in serviceTypeids)
                            {
                                var serviceType = await uow.ServiceTypesRepository.Get().Where(s => s.ServiceTypeId == serviceTypeId && s.Status == "Active").FirstOrDefaultAsync();
                                if (serviceType != null)
                                {
                                    var ServiceResult = await new ServiceAccess().GetSmallServices(serviceType.ServiceId, serviceType.ServiceTypeId, memberId, lang);
                                    if (ServiceResult != null)
                                    {
                                        categories.Services.Add(ServiceResult);
                                    }
                                }
                            }

                        }

                       
                    }


                    return categories;
                }
                else
                {
                    return null;
                }


            }
            catch (Exception ex)
            {

                return null;
            }
        }


        //public async Task<Category> GetCategorywiseRealstateService(int categoryId, int subcategoryId, string keyword = "", int size = 0, int count = 0,int memberId=0)
        //{

        //    try
        //    {
        //        if (count > 0)
        //        {
        //            count = count * size;
        //        }
        //        var serviceTypeids = new List<int>();

        //        var categories = await uow.CategoryRepository.Get().Where(s => s.Status == "Active" && s.CategoryId == categoryId).FirstOrDefaultAsync();


        //        if (categories != null)
        //        {


        //            if (!string.IsNullOrEmpty(categories.Image))
        //            {
        //                categories.Image = link + categories.Image;
        //            }

        //            if (!string.IsNullOrEmpty(keyword))
        //            {
        //                var searchWords = keyword.ToLower().Split(" ".ToCharArray(),
        //                    StringSplitOptions.RemoveEmptyEntries);

        //               var ids = await uow.ServiceCategoryRepository.Get().Where(s => s.CategoryId == categories.CategoryId && s.Status == "Active").OrderByDescending(s => s.ServiceTypeId).Select(s => s.ServiceTypeId).Skip(count).Take(size).ToListAsync();
        //                if (ids.Count() > 0 && ids != null)
        //                {
        //                    serviceTypeids = await uow.ServiceTypesRepository.Get().Where(s => searchWords.Any(t => s.ServiceTypeName.ToLower().Contains(t)) && ids.Contains(s.ServiceTypeId) && s.Status == "Active").OrderByDescending(s => s.ServiceTypeId).Select(s => s.ServiceTypeId).ToListAsync();
                           
        //                }
        //            }
        //            else
        //            {
        //                serviceTypeids = await uow.ServiceCategoryRepository.Get().Where(s => s.CategoryId == categories.CategoryId && s.Status == "Active").OrderByDescending(s => s.ServiceTypeId).Select(s => s.ServiceTypeId.Value).Skip(count).Take(size).ToListAsync();

        //            }

        //            if (serviceTypeids.Count() > 0 || serviceTypeids != null)
        //            {
        //                foreach (var serviceTypeId in serviceTypeids)
        //                {
        //                    var serviceType = await uow.ServiceTypesRepository.Get().Where(s => s.ServiceTypeId == serviceTypeId && s.Status == "Active").FirstOrDefaultAsync();
        //                    if (serviceType != null)
        //                    {
        //                        var ServiceResult = await new ServiceAccess().GetSmallRealEStateServices(serviceType.ServiceId, serviceType.ServiceTypeId,memberId);
        //                        if (ServiceResult != null)
        //                        {
        //                            categories.Services.Add(ServiceResult);
        //                        }
        //                    }
        //                }
        //            }


        //            return categories;
        //        }
        //        else
        //        {
        //            return null;
        //        }


        //    }
        //    catch (Exception ex)
        //    {

        //        return null;
        //    }
        //}


        public async Task<Category> GetCategorywiseOnlyService(int categoryId, string keyword = "", int size = 0, int count = 0, int memberId = 0, string lang = "en")
        {

            try
            {
                if (count > 0)
                {
                    count = count * size;
                }
                var serviceids = new List<int>();

                var categories = await uow.CategoryRepository.Get().Where(s => s.Status == "Active" && s.CategoryId == categoryId).FirstOrDefaultAsync();


                if (categories != null)
                {

                    categories.CategoryName = lang == "en" ? categories.CategoryName : categories.CategoryNameAr;
                    categories.CategoryDescription = lang == "en" ? categories.CategoryDescription : categories.CategoryDescriptionAr;
                    if (!string.IsNullOrEmpty(categories.Image))
                    {
                        categories.Image = link + categories.Image;
                    }

                    if (!string.IsNullOrEmpty(keyword))
                    {
                        var searchWords = keyword.ToLower().Split(" ".ToCharArray(),
                            StringSplitOptions.RemoveEmptyEntries);

                        var ids = await uow.ServiceCategoryRepository.Get().Where(s => s.CategoryId == categories.CategoryId && s.Status == "Active" && s.ServiceTypeId==null).OrderByDescending(s => s.ServiceId).Select(s => s.ServiceId).Skip(count).Take(size).ToListAsync();
                        if (ids.Count() > 0 && ids != null)
                        {
                            serviceids = await uow.ServiceRepository.Get().Where(s => searchWords.Any(t => s.Name.ToLower().Contains(t)) && ids.Contains(s.ServiceId) && s.Status == "Active").OrderByDescending(s => s.ServiceId).Select(s => s.ServiceId).ToListAsync();

                        }
                    }
                    else
                    {
                        serviceids = await uow.ServiceCategoryRepository.Get().Where(s => s.CategoryId == categories.CategoryId && s.Status == "Active").OrderByDescending(s => s.ServiceId).Select(s => s.ServiceId.Value).Skip(count).Take(size).ToListAsync();

                    }

                    if (serviceids.Count() > 0 || serviceids != null)
                    {
                        foreach (var serviceId in serviceids)
                        {
                           
                                var ServiceResult = await new ServiceAccess().GetSmallServices(serviceId, 0, memberId,lang);
                                if (ServiceResult != null)
                                {
                                    categories.Services.Add(ServiceResult);
                                }
                            
                        }
                    }


                    return categories;
                }
                else
                {
                    return null;
                }


            }
            catch (Exception ex)
            {

                return null;
            }
        }


        public async Task<Category> GetCategorywiseOnlyTypingandInsuranceService(int categoryId, string keyword = "", int size = 0, int count = 0, int memberId = 0, string lang = "en")
        {

            try
            {
                if (count > 0)
                {
                    count = count * size;
                }
                var serviceids = new List<int>();

                var categories = await uow.CategoryRepository.Get().Where(s => s.Status == "Active" && s.CategoryId == categoryId).FirstOrDefaultAsync();


                if (categories != null)
                {

                    categories.CategoryName = lang == "en" ? categories.CategoryName : categories.CategoryNameAr;
                    categories.CategoryDescription = lang == "en" ? categories.CategoryDescription : categories.CategoryDescriptionAr;
                    if (!string.IsNullOrEmpty(categories.Image))
                    {
                        categories.Image = link + categories.Image;
                    }

                    if (!string.IsNullOrEmpty(keyword))
                    {
                        var searchWords = keyword.ToLower().Split(" ".ToCharArray(),
                            StringSplitOptions.RemoveEmptyEntries);

                        var ids = await uow.ServiceCategoryRepository.Get().Where(s => s.CategoryId == categories.CategoryId && s.Status == "Active" && s.ServiceTypeId == null).OrderByDescending(s => s.ServiceId).Select(s => s.ServiceId).Skip(count).Take(size).ToListAsync();
                        if (ids.Count() > 0 && ids != null)
                        {
                            serviceids = await uow.ServiceRepository.Get().Where(s => searchWords.Any(t => s.Name.ToLower().Contains(t)) && ids.Contains(s.ServiceId) && s.Status == "Active" && s.ParentId == 0).OrderByDescending(s => s.ServiceId).Select(s => s.ServiceId).ToListAsync();

                        }
                    }
                    else
                    {
                        serviceids = await uow.ServiceCategoryRepository.Get().Where(s => s.CategoryId == categories.CategoryId && s.Status == "Active").OrderByDescending(s => s.ServiceId).Select(s => s.ServiceId.Value).Skip(count).Take(size).ToListAsync();

                    }

                    if (serviceids.Count() > 0 || serviceids != null)
                    {
                        foreach (var serviceId in serviceids)
                        {
                            var AreaFilteredList = await uow.ServiceRepository.Get().Where(p => p.ServiceId == serviceId && p.Status == "Active" && p.ParentId==0).Include(s => s.City).Include(s => s.Country).ToListAsync();

                            if (AreaFilteredList != null && AreaFilteredList.Count() > 0)
                            {
                                foreach (var AreaFiltered in AreaFilteredList)
                                {
                                    if (AreaFiltered != null)
                                    {
                                        var res = new ParentSmallServiceDetailsResponse();
                                        res.ServiceId = AreaFiltered.ServiceId;
                                        res.ServiceName = lang == "en" ? AreaFiltered.Name : AreaFiltered.NameAr;
                                        res.City = await new CityAccess().GetById(AreaFiltered.CityId.Value, lang);
                                        res.Country = await new CountryAccess().GetById(AreaFiltered.CountryId.Value, lang);
                                        res.CheckInTime = AreaFiltered.CheckInTime;
                                        res.CheckOutTime = AreaFiltered.CheckOutTime;
                                        
                                        res.ServiceAddress = AreaFiltered.Address;
                                        res.Description = lang == "en" ? AreaFiltered.Description : AreaFiltered.DescriptionAr;

                                        var ChlidListService = await uow.ServiceRepository.Get().Where(p => p.ParentId == AreaFiltered.ServiceId && p.Status == "Active" ).Select(s=>s.ServiceId).ToListAsync();
                                        if (ChlidListService != null && ChlidListService.Count() > 0)
                                        {
                                            foreach (var child in ChlidListService)
                                            {
                                                var ServiceResult = await new ServiceAccess().GetSmallServices(child, 0, memberId, lang);
                                                if (ServiceResult != null)
                                                {
                                                    res.ChildServices.Add(ServiceResult);
                                                }
                                            }
                                            
                                        }

                                        categories.MainServices.Add(res);

                                    }
                                }
                               
                            }

                        }


                        return categories;
                    }
                    else
                    {
                        return categories;
                    }
                }
                return categories;

                       


               }
            catch (Exception ex)
            {

                return null;
            }
        }




        public async  Task<List<Category>> GetParentChildWiseCategories(int featureCategoryId = 0,string type="All",bool isTop=false,bool isTrending=false,string lang="en")
        {
            try
            {
                var categoryList = await GetAll(featureCategoryId, type,lang);
                foreach (var category in categoryList.Where(t => t.ParentId == 0))
                {
                    // child brand
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
                                //break;
                            }
                            else
                            {
                                category.ChildCategories.Add(child);
                            }
                        }
                    }
                }
                if (isTop==true)
                {
                    categoryList = categoryList.Where(t => t.ParentId == 0 && t.IsTop==true).ToList();
                }
                else if(isTrending==true) 
                {
                    categoryList = categoryList.Where(t => t.ParentId == 0 && t.IsTrenbding == true).ToList();
                }
                else 
                {
                    categoryList = categoryList.Where(t => t.ParentId == 0).ToList();
                }
                
                return categoryList;
            }
            catch (Exception ex)
            {
                return new List<Category>();
            }
        }


        //public List<CategoryViewModel> GetParentChildWiseCategories()
        //{
        //    try
        //    {
        //        var categoryList = _categoryData.GetCategories();
        //        foreach (var category in categoryList.Where(t => t.ParentId.HasValue == false))
        //        {
        //            // child category
        //            var childCategory = categoryList.Where(t => (t.ParentId.HasValue && t.ParentId.Value == category.CategoryId));
        //            if (childCategory.Any())
        //            {
        //                foreach (var child in childCategory)
        //                {
        //                    var childCategorylist = categoryList.Where(t => (t.ParentId.HasValue && t.ParentId.Value == child.CategoryId));
        //                    if (childCategorylist.Any())
        //                    {
        //                        category.ChildCategory.Add(child);

        //                        category.ChildCategory[category.ChildCategory.Count - 1].ChildCategory.AddRange(childCategorylist);
        //                        //break;
        //                    }
        //                    else
        //                    {
        //                        category.ChildCategory.Add(child);
        //                    }
        //                }
        //            }
        //        }
        //        categoryList = categoryList.Where(t => t.ParentId.HasValue == false).ToList();
        //        return categoryList;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex.ToString());
        //        return new List<CategoryViewModel>();
        //    }
        //}


        public async Task<Category> GetParentChildWiseCategories(int categoryId, int featureCategoryId = 0,string lang="en")
        {
            try
            {
                var categoryList = await Get(categoryId, lang);
                //foreach (var category in categoryList.Where(t => t.ParentId == 0))
                //{
                    // child brand
                    var childCategories = await uow.CategoryRepository.Get().Where(t => t.ParentId > 0 && t.ParentId == categoryList.CategoryId && t.Status=="Active").ToListAsync();
                    if (childCategories.Any())
                    {
                        foreach (var child in childCategories)
                        {
                            var childCategorylist = childCategories.Where(t => (t.ParentId > 0 && t.ParentId == child.CategoryId));
                            if (childCategorylist.Any())
                            {
                            categoryList.ChildCategories.Add(child);

                            categoryList.ChildCategories[categoryList.ChildCategories.Count - 1].ChildCategories.AddRange(childCategorylist);
                                //break;
                            }
                            else
                            {
                            categoryList.ChildCategories.Add(child);
                            }
                        }
                    }

                if (categoryList.ChildCategories != null && categoryList.ChildCategories.Count > 0)
                {
                    foreach (var category in categoryList.ChildCategories)
                    {
                        category.CategoryName = lang == "en" ? category.CategoryName : category.CategoryNameAr;
                        category.CategoryDescription = lang == "en" ? category.CategoryDescription : category.CategoryDescriptionAr;
                        if (!string.IsNullOrEmpty(category.Image))
                        {
                            category.Image = link + category.Image;
                        }

                        if (!string.IsNullOrEmpty(category.Icon))
                        {
                            category.Icon = link + category.Icon;
                        }
                    }
                  
                }
                //}

                return categoryList;
            }
            catch (Exception ex)
            {
                return new Category();
            }
        }
    }
}