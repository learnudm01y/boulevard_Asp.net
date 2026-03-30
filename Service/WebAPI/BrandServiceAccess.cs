using Boulevard.BaseRepository;
using Boulevard.Models;
using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Boulevard.Service.WebAPI
{
    public class BrandServiceAccess
    {
        public IUnitOfWork uow;
        public BrandServiceAccess()
        {
            uow = new UnitOfWork();
        }

        string link = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/";

        public async Task<List<Brand>> GetAll(int featureCategoryId = 0,string type="All",bool isFeature=false,bool isTrending=false)

        {
            var Response = new List<Brand>();
            try
            {
                if (featureCategoryId == 0)
                {
                    if (type == "All")
                    {
                        Response = await uow.BrandRepository.Get().Where(s => s.Status == "Active" ).OrderByDescending(s=>s.BrandId).ToListAsync();
                    }
                    else
                    {
                        Response = await uow.BrandRepository.Get().Where(s => s.Status == "Active" ).OrderByDescending(s => s.BrandId).Take(6).ToListAsync();
                    }
                }
                else
                {
                    if (type == "All")
                    {
                        Response = await uow.BrandRepository.Get().Where(s => s.Status == "Active"  && s.FeatureCategoryId == featureCategoryId).OrderByDescending(s => s.BrandId).ToListAsync();
                    }
                    else
                    {
                        Response = await uow.BrandRepository.Get().Where(s => s.Status == "Active"  && s.FeatureCategoryId == featureCategoryId).OrderByDescending(s => s.BrandId).Take(6).ToListAsync();
                    }
                }
                if (Response.Count > 0 || Response != null)
                {
                    foreach (var item in Response)
                    {
                        if (!string.IsNullOrEmpty(item.Image))
                        {
                            item.Image = link + item.Image;
                        }
                       
                    }
                }
                if (isTrending == true)
                {
                    return Response.Where(s => s.IsTrenbding == true).ToList();
                }
                else if (isFeature == true)
                {
                    return Response.Where(s => s.IsFeature == true).ToList();
                }
                return Response;
            }
            catch (Exception ex)
            {
                //Log.Error(ex.ToString());
                return new List<Brand>();
            }
        }



        public async Task<Brand> GetBrand(int brandId)
        {
            try
            {
                var ss  = await uow.BrandRepository.Get().Where(s => s.Status == "Active" && s.BrandId== brandId).FirstOrDefaultAsync();
                if (ss != null)
                {
                    if (!string.IsNullOrEmpty(ss.Image))
                    {
                        ss.Image = link + ss.Image;
                    }
                    return ss;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {

                return null;
            }
        }
        public async Task<Brand> GetBrandWithProduct(int brandId, string keyword="", int size=0,int count=0,int memberId=0 )

        {
           
            try
            {
                if (count > 0)
                {
                    count = count * size;
                }
                var productids = new List<int>();

              

                var Response = await uow.BrandRepository.Get().Where(s => s.Status == "Active" && s.BrandId==brandId).FirstOrDefaultAsync();
                   
                  
                       
                    
               
                if ( Response != null)
                {
                   
                        if (!string.IsNullOrEmpty(Response.Image))
                        {
                        Response.Image = link + Response.Image;
                        }
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        var searchWords = keyword.ToLower().Split(" ".ToCharArray(),
                            StringSplitOptions.RemoveEmptyEntries);

                        productids = await uow.ProductRepository.Get().Where(s => searchWords.Any(t => s.ProductName.ToLower().Contains(t)) && s.BrandId == Response.BrandId && s.Status == "Active").OrderByDescending(s => s.ProductId).Select(s => s.ProductId).Skip(count).Take(size).ToListAsync();
                    }
                    else
                    {
                        productids = await uow.ProductRepository.Get().Where(s => s.BrandId == Response.BrandId && s.Status == "Active").OrderByDescending(s => s.ProductId).Select(s => s.ProductId).Skip(count).Take(size).ToListAsync();
                    }

                    if (productids.Count() > 0 || productids != null)
                    {
                        foreach (var productid in productids)
                        {
                            var productResult = await new ProductServiceAccess().getSmallDetailsProducts(productid, memberId);
                            if (productResult != null)
                            {
                                Response.Products.Add(productResult);
                            }
                        }
                    }


                }

                return Response;
            }
            catch (Exception ex)
            {
                //Log.Error(ex.ToString());
                return new Brand();
            }
        }
    }
}