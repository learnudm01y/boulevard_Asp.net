using Boulevard.RequestModels;
using Boulevard.Service.WebAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Razor.Parser;

namespace Boulevard.Controllers
{
    public class ServiceController : BaseController
    {
        public ServiceAccess _serviceAccess;
        public ServiceController()
        {
            _serviceAccess = new ServiceAccess();
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetPackaedges(int featureCategoryId, int pageNumber, int pageSize, string lang = "en")
        {
            try
            {
                var result = await _serviceAccess.GetPackaedges(featureCategoryId, pageNumber, pageSize,lang);
                if (result != null)
                {
                    return SuccessMessage(result);
                }
                else
                {
                    return ErrorMessage("No data found");
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
      
        [HttpPost]
        public async Task<IHttpActionResult> GetServices(ServiceSearchingRequest request)
        {
            try
            {
                var result = await _serviceAccess.GetServices(request);
                if (result != null)
                {
                    return SuccessMessage(result);
                }
                else
                {
                    return ErrorMessage("No data found");
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public async Task<IHttpActionResult> GetServicesByIdTypingandInsurance(int serviceId,int memberId=0,string lang="en")
        {
            try
            {
                var result = await _serviceAccess.GetServiceOnlyTypingandInsuranceService(serviceId,memberId,lang);
                if (result != null)
                {
                    return SuccessMessage(result);
                }
                else
                {
                    return ErrorMessage("No data found");
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<IHttpActionResult> GetServiceDetailsById(int serviceId, int memberId = 0, string lang = "en")
        {
            try
            {
                var result = await _serviceAccess.GetServiceDetailsById(serviceId,memberId,lang);
                if (result != null)
                {
                    return SuccessMessage(result);
                }
                else
                {
                    return ErrorMessage("No data found");
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
         public async Task<IHttpActionResult> GetSimilarDestination(int featureId, int serviceId, string lang = "en")
        {
            try
            {
                var result = await _serviceAccess.GetSimilarDestination(featureId, serviceId, lang);
                if (result != null)
                {
                    return SuccessMessage(result);
                }
                else
                {
                    return ErrorMessage("No data found");
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<IHttpActionResult> GetRelatedServices(int featureId, int serviceId, string lang = "en")
        {
            try
            {
                var result = await _serviceAccess.GetRelatedServices(featureId, serviceId, lang);
                if (result != null)
                {
                    return SuccessMessage(result);
                }
                else
                {
                    return ErrorMessage("No data found");
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<IHttpActionResult> SearchingServiceType(int featureCategoryId, string keyword = "", int size = 10, int count = 0, int memberId = 0, string lang = "en")
        {
            try
            {
                var result = await _serviceAccess.GetSearchingServices(featureCategoryId, keyword, size, count,memberId, lang);
                if (result != null)
                {
                    return SuccessMessage(result);
                }
                else
                {
                    return ErrorMessage("No data found");
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public async Task<IHttpActionResult> GetlatestProjectServicerealeastate(int memberid, int size, int count,int featureCategoryId,string lang = "en")
        {
            try
            {
                var result = await _serviceAccess.GetLatestprojectFromrealEstate( memberid,  size,  count, featureCategoryId,lang);
                if (result != null)
                {
                    return SuccessMessage(result);
                }
                else
                {
                    return ErrorMessage("No data found");
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public async Task<IHttpActionResult> GetlocationWiseRealEstate(int memberid, int size, int count, int featureCategoryId, string lang = "en")
        {
            try
            {
                var result = await _serviceAccess.GetLocationFromrealEstate(memberid, size, count, featureCategoryId, lang);
                if (result != null)
                {
                    return SuccessMessage(result);
                }
                else
                {
                    return ErrorMessage("No data found");
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public async Task<IHttpActionResult> GetFeatureWiseeRealEstate(int memberid, int size, int count, int featureCategoryId, string lang = "en")
        {
            try
            {
                var result = await _serviceAccess.GetFeatureFromrealEstate(memberid, size, count, featureCategoryId, lang);
                if (result != null)
                {
                    return SuccessMessage(result);
                }
                else
                {
                    return ErrorMessage("No data found");
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<IHttpActionResult> GetServiceDetailsrealStateById(int servicetypeId, int memberId = 0, string lang = "en")
        {
            try
            {
                var result = await _serviceAccess.GetServiceDetailsrealStateById(servicetypeId, memberId, lang);
                if (result != null)
                {
                    return SuccessMessage(result);
                }
                else
                {
                    return ErrorMessageNull("No data found",null);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public async Task<IHttpActionResult> GetFilterResponse(int featureCategoryId,  string lang = "en")
        {
            try
            {
                var result = await _serviceAccess.GetFilterResponse(featureCategoryId,lang);
                if (result != null)
                {
                    return SuccessMessage(result);
                }
                else
                {
                    return ErrorMessageNull("No data found", null);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetFilterWiseServiceRealEstate(FilterBodyRequest body)
        {
            try
            {
                var result = await _serviceAccess.GetFilterwiseRealstateService(body);
                if (result != null)
                {
                    return SuccessMessage(result);
                }
                else
                {
                    return ErrorMessageNull("No data found", null);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public async Task<IHttpActionResult> GetServiceAmenities(int featureCategoryId,string lang="en")
        {
            try
            {
                var result = await _serviceAccess.GetServiceAmenities(featureCategoryId, lang);
                if (result != null)
                {
                    return SuccessMessage(result);
                }
                else
                {
                    return ErrorMessageNull("No data found", null);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<IHttpActionResult> GetServiceNameTypingAndInsurance(int featureCategoryId, string lang = "en")
        {
            try
            {
                var result = await _serviceAccess.GetServiceNameTypingAndInsurance(featureCategoryId, lang);
                if (result != null)
                {
                    return SuccessMessage(result);
                }
                else
                {
                    return ErrorMessageNull("No data found", null);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        


    }
}
