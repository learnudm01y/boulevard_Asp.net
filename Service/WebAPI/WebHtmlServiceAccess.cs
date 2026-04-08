using Boulevard.BaseRepository;
using Boulevard.Models;
using Swashbuckle.SwaggerUi;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;

namespace Boulevard.Service.WebAPI
{
    public class WebHtmlServiceAccess
    {
        public IUnitOfWork uow;
        public WebHtmlServiceAccess()
        {
            uow = new UnitOfWork();
        }

        string link = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/";

        public async Task<List<WebHtml>> GetAll(string identifire="",int featureCategoryId=0, string lang = "en")

        {
            var Response = new List<WebHtml>();
            try
            {
                bool hasIdentifier = !string.IsNullOrWhiteSpace(identifire);

                if (featureCategoryId == 0)
                {
                    Response = hasIdentifier
                        ? await uow.WebHtmlRepository.Get().Where(s => s.Status == "Active" && s.Identifier == identifire).ToListAsync()
                        : await uow.WebHtmlRepository.Get().Where(s => s.Status == "Active").ToListAsync();
                }
                else
                {
                    Response = hasIdentifier
                        ? await uow.WebHtmlRepository.Get().Where(s => s.Status == "Active" && s.Identifier == identifire && s.FeatureCategoryId == featureCategoryId).ToListAsync()
                        : await uow.WebHtmlRepository.Get().Where(s => s.Status == "Active" && s.FeatureCategoryId == featureCategoryId).ToListAsync();
                }
                if (Response.Count > 0 || Response != null)
                {
                    foreach (var item in Response) 
                    {
                        if (!string.IsNullOrEmpty(item.PictureOne))
                        {
                            item.PictureOne = link + item.PictureOne;
                        }
                        if (!string.IsNullOrEmpty(item.PictureTwo))
                        {
                            item.PictureTwo = link + item.PictureTwo;
                        }
                        if (!string.IsNullOrEmpty(item.PictureThree))
                        {
                            item.PictureThree = link + item.PictureThree;
                        }
                        if (lang == "ar")
                        {
                            item.BigDetailsOne = item.BigDetailsOneAr;
                            item.BigDetailsTwo = item.BigDetailsTwoAr;
                            item.SmallDetailsOne = item.SmallDetailsOneAr;
                            item.SmallDetailsTwo = item.SmallDetailsTwoAr;
                            item.Title = item.TitleAr;
                            item.SubTitle = item.SubTitleAr;
                            if (!string.IsNullOrEmpty(item.PictureOneAr))
                            {
                               
                                item.PictureOne = link + item.PictureOneAr;
                            }
                        }
                       
                    }
                }

                return Response;
            }
            catch (Exception ex)
            {
                //Log.Error(ex.ToString());
                return new List<WebHtml>();
            }
        }
    }
}