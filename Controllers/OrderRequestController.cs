using Boulevard.Models;
using Boulevard.RequestModels;
using Boulevard.Service.WebAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Services.Description;

namespace Boulevard.Controllers
{
    public class OrderRequestController : BaseController
    {
        public OrderRequestServiceAccess _orderRequestServiceAccess;
        public FeatureCategoryServiceAccess _fService;
        public ProductServiceAccess _productService;
        public ServiceAccess _serviceAccess;
        public OrderRequestController() {
            _orderRequestServiceAccess = new OrderRequestServiceAccess();
            _fService = new FeatureCategoryServiceAccess();
            _productService = new ProductServiceAccess();
            _serviceAccess = new ServiceAccess();
        }
       


        public async Task<IHttpActionResult> OrderSubmit(OrderSubmitRequest model)
        {
            if (model.MemberId == 0 || model.MemberId==0)
            {
                return ErrorMessage("MemberId Or MemberAddressid Can not be 0");
            }
            var result = await _orderRequestServiceAccess.InsertOrder(model);
            if (result != null)
            {
                return SuccessMessage(result);
            }
            else
            {
                return ErrorMessageNull(result, null);
            }
        }



        public async Task<IHttpActionResult> GetProductStatus(int statusId,int orderId,string lang="en")
        {
            
            var result = await _productService.getStatusInfo(statusId, orderId, lang);
            if (result != null)
            {
                return SuccessMessage(result);
            }
            else
            {
                return ErrorMessageNull("Some Problem Occured", null);
            }
        }

        public async Task<IHttpActionResult> getOrdersByMember(int memberId, int featureCategoryId,int size = 10, int count = 0,string lang="en", string keyword = "")
        {
           
            var result = await _orderRequestServiceAccess.getOrderForMember(memberId, featureCategoryId, size,count, lang, keyword);
            if (result != null)
            {
                return SuccessMessage(result);
            }
            else
            {
                return ErrorMessage("No data found");
            }
        }

        public async Task<IHttpActionResult> getOrderServicesByMember(int memberId, int featureCategoryId, int size = 10, int count = 0,string keyword="")
        {

            var result = await _orderRequestServiceAccess.getOrderServicesForMember(memberId, featureCategoryId, size, count,keyword);
            if (result != null)
            {
                return SuccessMessage(result);
            }
            else
            {
                return ErrorMessage("No data found");
            }
        }

        public async Task<IHttpActionResult> InsertOrderRequestService(OrderRequestServiceRequest model)
        {
            if (model.MemberId == 0 || model.MemberId == 0)
            {
                return ErrorMessage("MemberId Or MemberAddressid Can not be 0");
            }
            var result = await _orderRequestServiceAccess.InsertOrderRequestService(model);
            if (result != null)
            {
                return SuccessMessage(result);
            }
            else
            {
                return ErrorMessageNull("Some Problem Occured",null);
            }
        }

        public async Task<IHttpActionResult> SearchAllProductAndService(string keyword = "", int size = 10, int count = 0, int memberId = 0)
        {
           
            var result = await _fService.GetAll();

            if (result != null && result.Count() > 0)
            {
                foreach (var singelresult in result)
                {
                    singelresult.Products = await _productService.GetProductBySearching(singelresult.FeatureCategoryId, keyword, size, count, memberId);
                    singelresult.Services = await _serviceAccess.GetSearchingServices(singelresult.FeatureCategoryId, keyword, size, count, memberId);
                }
            }
            if (result != null)
            {
                return SuccessMessage(result);
            }
            else
            {
                return ErrorMessageNull("Some Problem Occured", null);
            }
        }

        public async Task<IHttpActionResult> UpdatePaymentStatusService(int orderServiceId,string PaymentId)
        {
            
            var result = await _orderRequestServiceAccess.UpdatePaymentStatusService(orderServiceId, PaymentId);
            if (result ==true)
            {
                return SuccessMessage(result);
            }
            else
            {
                return ErrorMessageNull("Some Problem Occured", result);
            }
        }


    }
}
