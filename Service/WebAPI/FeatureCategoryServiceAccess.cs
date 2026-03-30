using Boulevard.BaseRepository;
using Boulevard.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Boulevard.Service.WebAPI
{
    public class FeatureCategoryServiceAccess
    {

        public IUnitOfWork uow;
        public FeatureCategoryServiceAccess()
        {
            uow = new UnitOfWork();
        }

        string link = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/";

        public async Task<List<FeatureCategory>> GetAll(string lang="en")

        {
            try
            {

                var Response = await uow.FeatureCategoryRepository.Get().Where(s=>s.IsDelete==false).ToListAsync();

                if (Response.Count > 0 || Response != null)
                {
                    foreach (var item in Response)
                    {
                        if (!string.IsNullOrEmpty(item.Image))
                        {

                            item.Image = link + item.Image;
                        }

                        item.Name = lang=="en"?item.Name:item.NameAr;
                       
                    }
                }
                return Response;
            }
            catch (Exception ex)
            {
                //Log.Error(ex.ToString());
                return new List<FeatureCategory>();
            }
        }


    }
}