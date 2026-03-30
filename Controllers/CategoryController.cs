using Boulevard.Models;
using Boulevard.Service.WebAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace Boulevard.Controllers
{
    public class CategoryController : BaseController
    {
        public CategoryServiceAccess _categoryService;
        public CategoryController()
        {
            _categoryService = new CategoryServiceAccess();
        }
        public async Task<IHttpActionResult> GetAll(int featureCategoryid=0,string type="All",bool isTop=false,bool isTrending=false,string lang="en")
        {
            var categories = await _categoryService.GetParentChildWiseCategories(featureCategoryid, type, isTop, isTrending, lang);
            if (categories != null)
            {
                return SuccessMessage(categories);
            }
            else
            {
                return ErrorMessage("No data found");
            }
        }

        public async Task<IHttpActionResult> GetCategoryById(int CategoryId, int featureCategoryid = 0, string lang = "en")
        {

            var categories = await _categoryService.GetParentChildWiseCategories(CategoryId, featureCategoryid, lang);
                
            if (categories != null)
            {
                return SuccessMessage(categories);
            }
            else
            {
                return ErrorMessage("No data found");
            }
        }

        public async Task<IHttpActionResult> GetParentCategorywiseProduct(int featureCategoryid = 0,int size=10,int count =0, int memberId = 0, string lang = "en")
        {

            var categories = await _categoryService.GetAllParent(featureCategoryid, size, count, memberId,lang);

            if (categories != null)
            {
                return SuccessMessage(categories);
            }
            else
            {
                return ErrorMessage("No data found");
            }
        }

        public async Task<IHttpActionResult> GetSingelCategorywiseProduct(int categoryId, string keyword="",int size = 0, int count = 0,int brandId=0, int memberId = 0,bool IsScheduled=false, string lang = "en",int productType=1)
        {

            var categories = await _categoryService.GetCategorywiseProduct(categoryId, keyword,size, count, brandId,memberId,IsScheduled,lang,productType);

            if (categories != null)
            {
                return SuccessMessage(categories);
            }
            else
            {
                return ErrorMessage("No data found");
            }
        }

        public async Task<IHttpActionResult> GetSingelCategorywiseService(int categoryId, string keyword = "", int size = 0, int count = 0, int memberId = 0, string lang = "en")
        {

            var categories = await _categoryService.GetCategorywiseService(categoryId, keyword, size, count, memberId,lang);

            if (categories != null)
            {
                return SuccessMessage(categories);
            }
            else
            {
                return ErrorMessage("No data found");
            }
        }

        public async Task<IHttpActionResult> GetSingelCategorywiseOnlyService(int categoryId, string keyword = "", int size = 0, int count = 0, int memberId = 0, string lang = "en")
        {

            var categories = await _categoryService.GetCategorywiseOnlyService(categoryId, keyword, size, count, memberId, lang);

            if (categories != null)
            {
                return SuccessMessage(categories);
            }
            else
            {
                return ErrorMessage("No data found");
            }
        }


        public async Task<IHttpActionResult> GetSingelCategorywiseOnlyTypingAndService(int categoryId, string keyword = "", int size = 0, int count = 0, int memberId = 0, string lang = "en")
        {

            var categories = await _categoryService.GetCategorywiseOnlyTypingandInsuranceService(categoryId, keyword, size, count, memberId, lang);

            if (categories != null)
            {
                return SuccessMessage(categories);
            }
            else
            {
                return ErrorMessage("No data found");
            }
        }


    }
}
