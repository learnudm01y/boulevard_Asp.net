using Boulevard.BaseRepository;
using Boulevard.Helper;
using Boulevard.Models;
using Boulevard.RequestModels;
using Boulevard.ResponseModel;
using Boulevard.Service.Admin;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.DynamicData;
using System.Web.Http.Results;
using System.Web.Security;

namespace Boulevard.Service.WebAPI
{
    public class OrderRequestServiceAccess
    {
        public IUnitOfWork uow;
        public CourierService course;
        public OrderRequestServiceAccess()
        {
            uow = new UnitOfWork();
            course = new CourierService();
        }
        string link = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/";
        public async Task<string> InsertOrder(OrderSubmitRequest model)
        {
            try
            {

                var orders = new OrderRequestProduct();


                //Guid.NewGuid();
                orders.ReadableOrderId = "Boulvard-" + await getIdFromOrderproduct(); 
                orders.MemberId = model.MemberId;
                orders.MemberAddressId = model.MemberAddressId;
                orders.OrderDateTime = Helper.DateTimeHelper.DubaiTime();
                orders.Comments = model.Comments == null ? "" : model.Comments;
                orders.PaymentMethodId = model.PaymentMethodId;
                orders.DeliveryCharge = model.DeliveryCharge;
                orders.TotalPrice = model.TotalPrice;
                orders.ServiceCharge = model.ServiceCharge;
                orders.Tip = model.Tip;
                orders.OrderStatusId = 1;
                orders.Status = "Active";
                orders.ProductType = model.ProductTypeId;
                orders.CreateBy = model.MemberId;
                orders.CreateDate = Helper.DateTimeHelper.DubaiTime();
                if (model.ProductTypeId == 1)
                {
                    orders.DeliveryDateTime = model.DeliveryDateTime.HasValue ? model.DeliveryDateTime.Value : Helper.DateTimeHelper.DubaiTime().AddMinutes(20);
                }
                else
                {
                    orders.DeliveryDateTime = model.DeliveryDateTime.HasValue ? model.DeliveryDateTime.Value : Helper.DateTimeHelper.DubaiTime().AddHours(24);
                }
               
                orders.PaymentTransectionId = model.PaytmentTransectionId;
                if (model.PaymentMethodId == 1)
                {
                    orders.PaymentStatus = "Pending";
                }
                else
                {
                    orders.PaymentStatus = "Success";
                }

                orders.FeatureCategoryId = model.featureCategoryId;



                var orderResult = await uow.OrderRequestProductRepository.Add(orders);
                foreach (var orderDetail in model.Details)
                {
                    var productDetails = new OrderRequestProductDetails();

                    productDetails.OrderRequestProductId = orderResult.OrderRequestProductId;
                    productDetails.ProductId = orderDetail.ProductId;
                    productDetails.ProductPriceId = orderDetail.ProductPriceId;
                    productDetails.Quantity = orderDetail.Quantity;
                    productDetails.GrossPrice = orderDetail.GrossPrice;
                    productDetails.Status = "Active";
                    productDetails.CreateBy = model.MemberId;
                    productDetails.CreateDate = Helper.DateTimeHelper.DubaiTime();
                    if (orderDetail.OfferInformationId == 0)
                    {
                        productDetails.OfferInformationId = null;
                    }
                    else
                    {
                        productDetails.OfferInformationId = orderDetail.OfferInformationId;
                    }

                    productDetails.IsMembershipOrder = orderDetail.IsMembershipOrder;

                    productDetails.DiscountType = orderDetail.MembershipDiscountType;

                    productDetails.DiscountAmount = orderDetail.MembershipDiscountAmount;

                    productDetails.MembershipId = orderDetail.MembershipId;



                    await uow.OrderRequestProductDetailsRepository.Add(productDetails);


                    await new StockLogDataAccess().StockOut(orderDetail.ProductId, orderResult.OrderRequestProductId, orderDetail.Quantity, model.MemberId,orderDetail.ProductPriceId);
                }

                if (orderResult != null)
                {
                    foreach (var pro in model.Details)
                    {
                        await new CartServiceAccess().AddOrRemoveCart(new CartListRequest { MemberId = model.MemberId, ProductId = Convert.ToInt32(pro.ProductId), FeatureCategoryId = model.featureCategoryId, Quantity = 0,ProductPriceId= pro.ProductPriceId });
                    }
                }

                var members = await uow.MemberRepository.Get().Where(s => s.MemberId == model.MemberId).FirstOrDefaultAsync();
                if (members != null)
                {
                    string M_Title = "Thank you For Your Order in Boulvard";

                    string M_Message = "Dear " + members.Name + ", Thanks for your order in the Boulvard.Your Order id is : "+ orders.ReadableOrderId+". You will be notified once our team verifies it. ";



                    await new AdminNotificationDataAccess().SaveNotification(model.MemberId, " A customer has placed a new shipment request. Order No : #" + orders.ReadableOrderId, "New Order", "Member", "Product", orders.OrderRequestProductId);

                    await new PushNotificationAccess().SendInvoiceMemberNotification(Convert.ToInt32(model.MemberId), M_Title, M_Message);

                    // Only call Jeeply if at least one product's brand has delivery enabled.
                    // Collect all ProductIds in this order, find their BrandIds, then check IsDeliveryEnabled.
                    var productIds = model.Details.Select(d => d.ProductId).Distinct().ToList();
                    var brandIds = await uow.ProductRepository.Get()
                        .Where(p => productIds.Contains(p.ProductId))
                        .Select(p => p.BrandId)
                        .Distinct()
                        .ToListAsync();
                    bool anyDeliveryEnabled = await uow.BrandRepository.Get()
                        .AnyAsync(b => brandIds.Contains(b.BrandId) && b.IsDeliveryEnabled);

                    if (anyDeliveryEnabled)
                    {
                        var courier = await course.CreateExpressShipmentAsync(orderResult.OrderRequestProductId);
                        if (courier.success == "error")
                        {
                            orderResult.CourierOrderResponse = courier.message;
                            orderResult.UpdateDate = Helper.DateTimeHelper.DubaiTime();
                            await uow.OrderRequestProductRepository.Edit(orderResult);
                        }
                    }
                    else
                    {
                        // All merchants in this order have delivery disabled — skip Jeeply entirely.
                        orderResult.CourierOrderResponse = "Delivery disabled for all merchants in this order.";
                        orderResult.UpdateDate = Helper.DateTimeHelper.DubaiTime();
                        await uow.OrderRequestProductRepository.Edit(orderResult);
                    }

                   
                
                }
                return orderResult.ReadableOrderId;
            }

            catch (Exception ex)
            {
                return ex.ToString();
            }

        }

        public async Task<List<OrderRequestProduct>> getOrderForMember(int memberId, int featureCategoryId, int size = 10, int count = 0,string lang="en",string keyword="")
        {
            try
            {
                if (count > 0)
                {
                    count = count * size;
                }
                //var orders = await uow.OrderRequestProductRepository.Get().Where(s => s.MemberId == memberId && s.FeatureCategoryId == featureCategoryId).OrderByDescending(s => s.OrderRequestProductId).Skip(count).Take(size).ToListAsync();
                var query = uow.OrderRequestProductRepository.Get()
    .Where(s => s.MemberId == memberId
             && s.FeatureCategoryId == featureCategoryId);

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    query = query.Where(s => s.ReadableOrderId.Contains(keyword));
                }

                var orders = await query
                    .OrderByDescending(s => s.OrderRequestProductId)
                    .Skip(count)
                    .Take(size)
                    .ToListAsync();

                if (orders != null && orders.Count() > 0)
                {
                    foreach (var order in orders)
                    {
                        
                            order.OrderStatushistory = await new ProductServiceAccess().getStatusInfo(order.OrderStatusId.Value, order.OrderRequestProductId, lang);
                        
                        var orderDetails = await uow.OrderRequestProductDetailsRepository.Get().Where(s => s.OrderRequestProductId == order.OrderRequestProductId).ToListAsync();
                        if (orderDetails != null && orderDetails.Count() > 0)
                        {
                            foreach (var orderdetail in orderDetails)
                            {
                                var DetailsList = await new ProductServiceAccess().getSmallDetailsProducts(orderdetail.ProductId, memberId, true);
                                if (DetailsList != null)
                                {
                                    DetailsList.Quantity = orderdetail.Quantity;
                                    DetailsList.ProductPrice = Convert.ToDecimal(orderdetail.GrossPrice);
                                    if (orderdetail.OfferInformationId != null && orderdetail.OfferInformationId > 0)
                                    {
                                        DetailsList.OfferInformation = await uow.OfferInformationRepository.Get().Where(s => s.OfferInformationId == orderdetail.OfferInformationId).FirstOrDefaultAsync();
                                        DetailsList.IsOfferDiscount = true;

                                    }
                                    if (orderdetail.MembershipId != null && orderdetail.MembershipId > 0)
                                    {
                                        DetailsList.MembershipInformation = await uow.MemberShipRepository.Get().Where(s => s.MemberShipId == orderdetail.MembershipId).FirstOrDefaultAsync();
                                        DetailsList.IsMembershipOrder = true;

                                    }

                                    order.OrderRequestDetails.Add(DetailsList);
                                }

                            }
                        }

                        if (order.ProductType > 0)
                        {
                            order.ProductTypeName = await uow.ProductTypeMasterRepository.Get().Where(s => s.ProductTypeId == order.ProductType).Select(s => s.Name).FirstOrDefaultAsync();
                        }
                      
                    }
                    return orders;
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
        public async Task<List<OrderRequestServiceResponse>> getOrderServicesForMember(int memberId, int featureCategoryId, int size = 10, int count = 0,string keyword="")
        {
            try
            {
                if (count > 0)
                {
                    count = count * size;
                }
                List<OrderRequestServiceResponse> response = new List<OrderRequestServiceResponse>();
                //var orders = await uow.OrderRequestServiceRepository.Get().Where(s => s.MemberId == memberId && s.FeatureCategoryId == featureCategoryId).OrderByDescending(p => p.OrderRequestServiceId).Skip(count).Take(size).Include(p => p.OrderRequestServiceDetailList).Include(p => p.Member).ToListAsync();
                var query = uow.OrderRequestServiceRepository.Get()
    .Where(s => s.MemberId == memberId
             && s.FeatureCategoryId == featureCategoryId);

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    query = query.Where(s => s.BookingId.Contains(keyword));
                }

                var orders = await query
                    .Include(p => p.OrderRequestServiceDetailList)
                    .Include(p => p.Member)
                    .OrderByDescending(p => p.OrderRequestServiceId)
                    .Skip(count)
                    .Take(size)
                    .ToListAsync();
                if (orders != null && orders.Count() > 0)
                {
                    foreach (var order in orders)
                    {
                        OrderRequestServiceResponse obj = new OrderRequestServiceResponse();
                        obj.OrderId = order.OrderRequestServiceId;
                        obj.BookingId = order.BookingId;
                        obj.BookingMemberType = order.BookingMemberType;
                        if (string.IsNullOrEmpty(order.FirstName) && string.IsNullOrEmpty(order.LastName))
                        {
                            if (order.Member != null)
                            {
                                obj.MemberName = order.Member.Name;
                            }
                        }
                        else
                        {
                            obj.MemberName = order.FirstName + " " + order.LastName;
                        }
                        obj.InTime = order.InTime;
                        obj.OutTime = order.OutTime;
                        obj.Email = order.Email;
                        obj.FromAirportId = order.FromAirportId;
                        obj.ToAirportId = order.ToAirportId;
                        obj.Phone = order.PhoneCode + " " + order.PhoneNo;
                        obj.TotalPrice = order.TotalPrice;
                        obj.ExtraCharge = order.ExtraCharge;
                        obj.BookingDate = order.BookingDate.ToShortDateString();
                        obj.BookingTime = order.BookingTime;
                        obj.PaymentStatus = order.PaymentStatus;
                        obj.ServicePrice = order.ServiceCharge;
                        obj.QuotedPrice = order.QuotedPrice;
                        obj.IsApprovedByAdmin = order.IsApprovedByAdmin;
                        obj.IsPackage = order.IsPackage;
                        if (!string.IsNullOrEmpty(order.QuotationFileLink))
                        {
                            obj.QuotationFileLink = link + order.QuotationFileLink;
                        }
                        obj.QuotationNote = order.QuotationNote;

                        obj.IsApprovalSystem = order.FeatureCategory.IsWaitForApproval;
                        obj.IsQuoteSystem = order.FeatureCategory.IsQuoteEnable;

                        var memberaddress = await uow.MemberAddressRepository.Get().Where(s => s.MemberAddressId == order.MemberAddressId).FirstOrDefaultAsync();
                        if (memberaddress != null)
                        {
                            obj.MemberAddress = memberaddress;

                        }

                        var vehical = await uow.MemberVehicalInfoRepository.Get().Where(s => s.MemberVehicalInfoId == order.MemberVehicalInfoId).FirstOrDefaultAsync();
                        if (vehical != null)
                        {
                            obj.VehicalInfo = vehical;

                        }


                        var orderDetails = await uow.OrderRequestServiceDetailsRepository.Get().Where(s => s.OrderRequestServiceId == order.OrderRequestServiceId).Include(p => p.ServiceType).Include(p => p.Service).ToListAsync();
                        if (orderDetails != null && orderDetails.Count() > 0)
                        {
                            foreach (var orderdetail in orderDetails)
                            {
                                OrderRequestServiceDetailsResponse dtl = new OrderRequestServiceDetailsResponse();
                                dtl.ServiceId = orderdetail.ServiceId;
                                dtl.ServiceTypeId = orderdetail.ServiceTypeId;
                                dtl.ServiceName = orderdetail.Service.Name;
                                if (orderdetail.ServiceType != null)
                                {
                                    dtl.ServiceType = orderdetail.ServiceType.ServiceTypeName;
                                }
                                dtl.GrossPrice = orderdetail.GrossPrice;
                                dtl.DiscountAmount = orderdetail.DiscountAmount;
                                dtl.DiscountType = orderdetail.DiscountType;

                                if (orderdetail.OfferInformationId != null && orderdetail.OfferInformationId > 0)
                                {
                                    dtl.OfferInformation = await uow.OfferInformationRepository.Get().Where(s => s.OfferInformationId == orderdetail.OfferInformationId).FirstOrDefaultAsync();
                                    dtl.IsOfferDiscount = true;

                                }
                                if (orderdetail.MembershipId != null && orderdetail.MembershipId > 0)
                                {
                                    dtl.MembershipInformation = await uow.MemberShipRepository.Get().Where(s => s.MemberShipId == orderdetail.MembershipId).FirstOrDefaultAsync();
                                    dtl.IsMembershipOrder = true;

                                }
                                obj.OrderRequestServiceDetailList.Add(dtl);
                            }
                        }



                        response.Add(obj);

                    }
                    return response;
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
        public async Task<string> InsertOrderRequestService(OrderRequestServiceRequest model)
        {
            try
            {
                OrderRequestService node = new OrderRequestService();
                node.BookingId =   "Boulvard-" + await getIdFromOrderService(); ;
                node.BookingMemberType = model.BookingMemberType;
                node.MemberId = model.MemberId;
                node.MemberNameTitle = model.MemberNameTitle;
                node.FirstName = model.FirstName;
                node.LastName = model.LastName;
                node.Email = model.Email;
                node.PhoneCode = model.PhoneCode;
                node.PhoneNo = model.PhoneNo;
                
                node.ExtraCharge = model.ExtraCharge;
                node.InTime = model.InTime;
                node.OutTime = model.OutTime;
                node.IsPackage = model.IsPackage;
                node.FromAirportId = model.FromAirportId;
                node.ToAirportId = model.ToAirportId;
                node.DeliveryCharge = model.DeliveryCharge;
                node.ServiceCharge = model.ServiceCharge;
                //node.PaymentMethodId = 2;
                node.PaymentTransectionId = model.PaytmentTransectionId;
                if (model.PaymentMethodId == 1)
                {
                    node.PaymentStatus = "Pending";
                }
                else
                {
                    node.PaymentStatus = "Success";
                }
                if ((model.featureCategoryId == 1 || model.featureCategoryId == 12 || model.featureCategoryId == 9 || model.featureCategoryId == 11) && model.IsPackage == false)
                {
                    node.PaymentStatus = "Pending";
                }
                //else
                //{
                //    node.PaymentStatus = "Success";
                //}

                

                node.MemberAddressId = model.MemberAddressId;
                if (model.MemberVehicalInfoId == 0)
                {
                    node.MemberVehicalInfoId = null;
                }
                else
                {
                    node.MemberVehicalInfoId = model.MemberVehicalInfoId;
                }
                if (!string.IsNullOrEmpty(model.BookingDate))
                {
                    node.BookingDate = DateTime.Parse(model.BookingDate);

                }
                else
                {
                    node.BookingDate = DateTimeHelper.DubaiTime();
                }
                node.BookingTime = model.BookingTime;
                node.BookingStatus = "Active";
                node.FeatureCategoryId = model.featureCategoryId;
                if (!string.IsNullOrEmpty(model.PassportCopy))
                {
                    node.PassportCopy = model.PassportCopy;
                }

                node.TotalPrice = model.TotalPrice;
                node = await uow.OrderRequestServiceRepository.Add(node);

                foreach (var obj in model.OrderRequestServiceDetailList)
                {
                    OrderRequestServiceDetails dtls = new OrderRequestServiceDetails();
                    dtls.ServiceId = obj.ServiceId;
                    if (obj.ServiceTypeId > 0)
                    {
                        dtls.ServiceTypeId = obj.ServiceTypeId;
                    }
                    else
                    {
                        dtls.ServiceTypeId = null;
                    }
                    dtls.Quantity = obj.Quantity;
                    dtls.GrossPrice = obj.GrossPrice;
                    dtls.OrderRequestServiceId = node.OrderRequestServiceId;
                    if (obj.OfferInformationId == 0)
                    {
                        dtls.OfferInformationId = null;
                    }
                    else
                    {
                        dtls.OfferInformationId = obj.OfferInformationId;
                    }

                    dtls.IsMembershipOrder = obj.IsMembershipOrder;

                    dtls.DiscountType = obj.MembershipDiscountType;

                    dtls.DiscountAmount = obj.MembershipDiscountAmount;

                    dtls.MembershipId = obj.MembershipId;
                    await uow.OrderRequestServiceDetailsRepository.Add(dtls);
                }


                        await new CartServiceAccess().RemoveCartService( Convert.ToInt32(model.MemberId),  model.featureCategoryId);
                
                string M_Title = "Boulvard";

                string M_Message = "Dear " + model.FirstName + " " + model.LastName + ", Thanks for posting in the Boulvard.Your Service Order Id is : "+node.BookingId+". You will be notified once our team verifies it. ";



                await new PushNotificationAccess().SendInvoiceMemberNotification(Convert.ToInt32(model.MemberId), M_Title, M_Message);
                await new AdminNotificationDataAccess().SaveNotification(Convert.ToInt32(model.MemberId), " A customer has placed a new Service request. Order No : #" + node.BookingId, "New Service Request", "Member", "Service", node.OrderRequestServiceId);

                return node.BookingId;
            }

            catch (Exception ex)
            {
                return null;
            }

        }

        public async Task<bool> UpdatePaymentStatusService(int orderServiceId, string paymentId)
        {
            try
            {
                var result = await uow.OrderRequestServiceRepository.Get().Where(s => s.OrderRequestServiceId == orderServiceId).FirstOrDefaultAsync();

                result.PaymentStatus = "Success";
                result.PaymentTransectionId = paymentId;
                await uow.OrderRequestServiceRepository.Edit(result);
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        public async Task<string>  getIdFromOrderService()
        {
            try
            {
               
                string output = "";
                var digit = "";
                var digitInt = 0;

                var OrderId = await uow.OrderRequestServiceRepository.Get().OrderByDescending(s => s.OrderRequestServiceId).Select(s => s.BookingId).FirstOrDefaultAsync();
                if (OrderId != null)
                {
                    digit = OrderId.Replace("Boulvard-", "");
                    digitInt = Convert.ToInt32(digit) + 1;

                }
                if (digitInt.ToString().Length == 1)
                {
                    output = "00000" + digitInt.ToString();

                }
                else if (digitInt.ToString().Length == 2)
                {
                    output = "0000" + digitInt.ToString();

                }
                else if (digitInt.ToString().Length == 3)
                {
                    output = "000" + digitInt.ToString();
                }
                else if (digitInt.ToString().Length == 4)
                {
                    output = "00" + digitInt.ToString();
                }
                else if (digitInt.ToString().Length == 5)
                {
                    output = "0" + digitInt.ToString();
                }
                else
                {
                    output = digitInt.ToString();

                }


                return output;
            }
            catch (Exception ex)
            {
                return "999999";


            }

        }


        public async Task<string> getIdFromOrderproduct()
        {
            try
            {

                string output = "";
                var digit = "";
                var digitInt = 0;

                var OrderId = await uow.OrderRequestProductRepository.Get().OrderByDescending(s => s.OrderRequestProductId).Select(s => s.ReadableOrderId).FirstOrDefaultAsync();
                if (OrderId != null)
                {
                    digit = OrderId.Replace("Boulvard-", "");
                    digitInt = Convert.ToInt32(digit) + 1;

                }
                if (digitInt.ToString().Length == 1)
                {
                    output = "00000" + digitInt.ToString();

                }
                else if (digitInt.ToString().Length == 2)
                {
                    output = "0000" + digitInt.ToString();

                }
                else if (digitInt.ToString().Length == 3)
                {
                    output = "000" + digitInt.ToString();
                }
                else if (digitInt.ToString().Length == 4)
                {
                    output = "00" + digitInt.ToString();
                }
                else if (digitInt.ToString().Length == 5)
                {
                    output = "0" + digitInt.ToString();
                }
                else
                {
                    output = digitInt.ToString();

                }


                return output;
            }
            catch (Exception ex)
            {
                return "999999";


            }

        }

    }
}