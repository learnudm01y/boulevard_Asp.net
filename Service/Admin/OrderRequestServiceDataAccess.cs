using Boulevard.BaseRepository;
using Boulevard.Models;
using Boulevard.RequestModels;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Boulevard.Service.Admin
{
    public class OrderRequestServiceDataAccess
    {
        public IUnitOfWork uow;

        public OrderRequestServiceDataAccess()
        {
            uow = new UnitOfWork();
        }

        /// <summary>
        /// Get All from Order Request Service
        /// </summary>
        /// <returns></returns>
        public async Task<List<OrderRequestService>> GetAll()
        {
            try
            {
                var dataModel = await uow.OrderRequestServiceRepository.GetAll().OrderByDescending(t => t.OrderRequestServiceId).ToListAsync();
                foreach (var item in dataModel)
                {
                    item.Member = await uow.MemberRepository.Get().FirstOrDefaultAsync(s => s.MemberId == item.MemberId);
                }
                return dataModel;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        public async Task<List<OrderRequestService>> GetAllByFeatureCategory(string fCatagoryKey)
        {
            try
            {
                var dataModel = new List<OrderRequestService>();
                if (!string.IsNullOrEmpty(fCatagoryKey))
                {
                    var fCatagory = await uow.FeatureCategoryRepository.Get().FirstOrDefaultAsync(a => a.FeatureCategoryKey.ToString() == fCatagoryKey);
                    dataModel = await uow.OrderRequestServiceRepository.GetAll().Where(t => t.FeatureCategoryId == fCatagory.FeatureCategoryId).OrderByDescending(t => t.OrderRequestServiceId).ToListAsync();
                }
                else
                {
                    dataModel = await uow.OrderRequestServiceRepository.GetAll().OrderByDescending(t => t.OrderRequestServiceId).ToListAsync();
                }
                foreach (var item in dataModel)
                {
                    item.Member = await uow.MemberRepository.Get().FirstOrDefaultAsync(s => s.MemberId == item.MemberId);
                    item.IsPackage = item.IsPackage;
                }
                return dataModel;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Get Order Request Service By Id
        /// </summary>
        /// <param name="modelId"></param>
        /// <returns></returns>
        public async Task<OrderRequestService> GetOrderRequestServiceById(int modelId)
        {
            try
            {
                return await uow.OrderRequestServiceRepository.GetAll(a => a.OrderRequestServiceId == modelId).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        //public async Task<OrderRequestServiceDetails> Details(int id)
        //{
        //    try
        //    {
        //        var orderRequest = await uow.OrderRequestServiceDetailsRepository.Get().FirstOrDefaultAsync(a => a.OrderRequestServiceId == id);
        //        var service = await uow.ServiceRepository.Get().FirstOrDefaultAsync(a => a.ServiceId == orderRequest.ServiceId);
        //        orderRequest.Service = service != null ? service : new Models.Service();
        //        var odderInfo = await uow.OfferInformationRepository.Get().FirstOrDefaultAsync(o => o.FeatureCategoryId == service.FeatureCategoryId);
        //        orderRequest.OfferInformation = odderInfo != null ? odderInfo : new OfferInformation();
        //        orderRequest.ServiceType = await uow.ServiceTypesRepository.Get().FirstOrDefaultAsync(a => a.ServiceTypeId == orderRequest.ServiceTypeId);
        //        var orderRequestService = await uow.OrderRequestServiceRepository.Get().Include(m => m.Member).FirstOrDefaultAsync(a => a.OrderRequestServiceId == id);
        //        orderRequest.OrderRequestService = orderRequestService != null ? orderRequestService : new OrderRequestService();
        //        orderRequest.ServiceTypeList = await uow.ServiceTypesRepository.GetAll().Where(s => s.ServiceId == orderRequest.ServiceId).ToListAsync();
        //        var vehicalInfo = await uow.MemberVehicalInfoRepository.Get().Include(b => b.Brand).Include(a => a.VehicalModel).Include(a => a.Member).FirstOrDefaultAsync(a => a.MemberId == orderRequestService.MemberId);
        //        if (vehicalInfo != null)
        //        {
        //            orderRequest.MemberVehicalInfo = vehicalInfo;
        //        }
        //        if (orderRequest.ServiceTypeList != null)
        //        {
        //            foreach (var details in orderRequest.ServiceTypeList)
        //            {
        //                var serviceModel = await uow.ServiceRepository.Get().FirstOrDefaultAsync(s => s.ServiceId == details.ServiceId);
        //                details.Service = serviceModel != null ? serviceModel : new Models.Service();
        //                //var offerInformationId = await uow.OrderRequestServiceDetailsRepository.Get().Where(a => a.ServiceTypeId == details.ServiceTypeId).Select(a => a.OfferInformationId).FirstOrDefaultAsync();
        //                var offerInformation = await uow.OrderRequestServiceDetailsRepository.Get().Where(a => a.OrderRequestServiceId == orderRequestService.OrderRequestServiceId).FirstOrDefaultAsync();

        //                if (offerInformation.OfferInformationId != null)
        //                {
        //                    var offerName = await uow.OfferInformationRepository.Get().Where(o => o.OfferInformationId == offerInformation.OfferInformationId).Select(s => s.Title).FirstOrDefaultAsync();
        //                    details.OfferName = offerName != null ? offerName : "No Offer";
        //                    details.DiscountAmount = offerInformation.DiscountAmount != 0 ? offerInformation.DiscountAmount : 0;
        //                    details.DiscountType = offerInformation.DiscountType != null ? offerInformation.DiscountType : null;
        //                    //details.IsOffer = true;
        //                    orderRequest.IsOffer = true;
        //                }
        //                if (offerInformation.IsMembershipOrder == true)
        //                {
        //                    var memberShip = await uow.MemberShipRepository.Get().Where(a => a.MemberShipId == offerInformation.MembershipId).FirstOrDefaultAsync();
        //                    if (memberShip != null)
        //                    {
        //                        details.MemberShipName = memberShip.Title != null ? memberShip.Title : null;
        //                        details.DiscountAmount = offerInformation.DiscountAmount != 0 ? offerInformation.DiscountAmount : 0;
        //                        //details.IsMemberShipOffer = true;
        //                        orderRequest.IsMemberShipOffer = true;
        //                    }
        //                }
        //                //orderRequest.IsMemberShipOffer = details.IsMembershipOrder;
        //            }
        //        }
        //        return orderRequest;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex.ToString());
        //        throw;
        //    }
        //}

        public async Task<OrderRequestService> Details(int id)
        {
            try
            {
                var orderRequest = await uow.OrderRequestServiceRepository.Get().Where(a => a.OrderRequestServiceId == id).Include(s => s.Member).Include(s => s.FeatureCategory).FirstOrDefaultAsync();
                if (orderRequest != null)
                {
                    if (orderRequest.MemberVehicalInfoId != null && orderRequest.MemberVehicalInfoId > 0)
                    {
                        var vehicalInfo = await uow.MemberVehicalInfoRepository.Get().Include(b => b.Brand).Include(a => a.VehicalModel).FirstOrDefaultAsync(a => a.MemberVehicalInfoId == orderRequest.MemberVehicalInfoId);
                        if (vehicalInfo != null)
                        {
                            orderRequest.MemberVehicalInfo = vehicalInfo;
                        }
                    }


                    var orderRequestDetails = await uow.OrderRequestServiceDetailsRepository.Get().Where(s => s.OrderRequestServiceId == id).ToListAsync();
                    if (orderRequestDetails != null && orderRequestDetails.Count() > 0)
                    {
                        foreach (var details in orderRequestDetails)
                        {
                            var service = await uow.ServiceRepository.Get().FirstOrDefaultAsync(a => a.ServiceId == details.ServiceId);
                            if (service != null)
                            {
                                details.Service = service;
                            }

                            var serviceType = await uow.ServiceTypesRepository.Get().FirstOrDefaultAsync(a => a.ServiceTypeId == details.ServiceTypeId);
                            details.ServiceType = serviceType != null ? serviceType : new ServiceType();

                            if (details.OfferInformationId != null && details.OfferInformationId > 0)
                            {
                                var offerInfo = await uow.OfferInformationRepository.Get().FirstOrDefaultAsync(o => o.OfferInformationId == details.OfferInformationId);
                                if (offerInfo != null)
                                {
                                    details.OfferName = offerInfo.Title;

                                    details.IsOffer = true;
                                }
                            }
                            else if (details.MembershipId != null && details.MembershipId > 0)
                            {

                                var memberShip = await uow.MemberShipRepository.Get().Where(a => a.MemberShipId == details.MembershipId).FirstOrDefaultAsync();
                                if (memberShip != null)
                                {
                                    details.MemberShipName = memberShip.Title != null ? memberShip.Title : null;

                                    details.IsMemberShipOffer = true;
                                }
                            }

                            orderRequest.OrderRequestServiceDetailList.Add(details);
                        }
                        //orderRequest.OrderRequestServiceDetailList = orderRequest.OrderRequestServiceDetailList != null ? orderRequest.OrderRequestServiceDetailList : new List<OrderRequestServiceDetails>();
                    }
                }

                return orderRequest;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        public async Task Approved(int orderRequestServiceId)
        {
            try
            {
                var orderRequestService = await uow.OrderRequestServiceRepository.Get().Where(t => t.OrderRequestServiceId == orderRequestServiceId).FirstOrDefaultAsync();
                if (orderRequestService == null)
                {
                    //var node = new OrderRequestService();
                    orderRequestService.IsApprovedByAdmin = true;
                    await uow.OrderRequestServiceRepository.Edit(orderRequestService);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}