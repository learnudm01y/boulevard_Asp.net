using Boulevard.BaseRepository;
using Boulevard.Helper;
using Boulevard.Models;
using Boulevard.RequestModels;
using Boulevard.ResponseModel;
using Boulevard.Service.Admin;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using static Boulevard.RequestModels.DeliveryOrderForCourierRequest;

namespace Boulevard.Service.WebAPI
{
    public class CourierService
    {
        private readonly HttpClient _client;
        public IUnitOfWork uow;
        public CourierService()
        {
            _client = new HttpClient();
            uow = new UnitOfWork();
        }

        public async Task<CreateOrderCourierResponse> CreateExpressShipmentAsync(int orderId)
        {
            try
            {
                var shipmentResponse = new CreateOrderCourierResponse();
                var orderInfo = await uow.OrderRequestProductRepository.GetById(orderId);
                if (orderInfo != null)
                {
                    // Guard: check if any product in this order belongs to a brand with delivery enabled.
                    // If all brands have delivery disabled, skip Jeeply entirely.
                    var productIds = await uow.OrderRequestProductDetailsRepository.Get()
                        .Where(d => d.OrderRequestProductId == orderId)
                        .Select(d => d.ProductId)
                        .Distinct()
                        .ToListAsync();
                    var brandIds = await uow.ProductRepository.Get()
                        .Where(p => productIds.Contains(p.ProductId))
                        .Select(p => p.BrandId)
                        .Distinct()
                        .ToListAsync();
                    bool anyDeliveryEnabled = await uow.BrandRepository.Get()
                        .AnyAsync(b => brandIds.Contains(b.BrandId) && b.IsDeliveryEnabled);

                    if (!anyDeliveryEnabled)
                    {
                        shipmentResponse.success = "skipped";
                        shipmentResponse.message = "Delivery is disabled for all merchants in this order.";
                        return shipmentResponse;
                    }

                    var productType = await uow.ProductTypeMasterRepository.Get().Where(s => s.ProductTypeId == orderInfo.ProductType).FirstOrDefaultAsync();
                    var reciveMemberInfo = await uow.MemberRepository.GetById(orderInfo.MemberId);
                    var deliveryaddress = await uow.MemberAddressRepository.Get().Where(s => s.MemberAddressId == orderInfo.MemberAddressId).FirstOrDefaultAsync();
                    
                    if (productType != null && deliveryaddress!=null)
                    {
                            var order = new DeliveryOrderForCourierRequest();
                        
                            order.order_type = "Others";
                            order.pickup_building = productType.Pickup_Building;
                            order.pickup_street = productType.Pickup_Street;
                            order.pickup_area = productType.Pickup_Area;
                            order.pickup_city = productType.Pickup_City/*"Dubai"*/;
                            order.destination_name = "";
                            order.destination_address = deliveryaddress.AddressLine1;
                            order.destination_building = deliveryaddress.AddressLine1;
                            order.destination_street = deliveryaddress.AddressLine1;
                            order.destination_area = deliveryaddress.AddressLine1;
                        order.destination_city = /*"Dubai";*/await uow.CityRepository.Get().Where(s => s.CityId == deliveryaddress.CityId).Select(s => s.CityName).FirstOrDefaultAsync();
                        if (orderInfo.PaymentMethodId == 1)
                        {
                            order.payment_method = "COD";
                            order.cod_amount = orderInfo.TotalPrice;
                            order.tip_amount = orderInfo.Tip;
                        }
                        else
                        {
                            order.payment_method = "Prepaid";
                            order.cod_amount = 0;
                            order.tip_amount = orderInfo.Tip;
                        }
                           
                            order.no_of_packages = 1;
                          
                            order.pickup_contact_phone_country_code = "+971";

                            order.pickup_contact_phone = productType.PickUpContactNo;
                            order.recipient_contact_phone_country_code = reciveMemberInfo.PhoneCode;
                            order.recipient_contact_phone = reciveMemberInfo.PhoneNumber;
                            order.pickup_name = productType.PickUpContactName;
                            order.recipient_name = reciveMemberInfo.Name;
                            order.extra_info = orderInfo.Comments;
                        order.planned_start_time = orderInfo.OrderDateTime.AddMinutes(2).ToString("yyyy-MM-dd HH:mm");/*DateTime.Now.AddMinutes(2).ToString("yyyy-MM-dd HH:mm");*/
                        order.planned_delivery_time = orderInfo.DeliveryDateTime.AddHours(4).ToString("yyyy-MM-dd HH:mm");/*DateTime.Now.AddDays(2).ToString("yyyy-MM-dd HH:mm");*/
                            order.vehicle_type = "Bike";
                            order.pick_lat = productType.Latitute;
                            order.pick_long = productType.Longitute;
                            order.drop_lat = deliveryaddress.latitude;
                            order.drop_long = deliveryaddress.longitude;
                        

                        // Convert C# object to JSON string
                        var jsonBody = JsonSerializer.Serialize(order);

                        var request = new HttpRequestMessage(HttpMethod.Post,
                            "https://demo.jeebly.com/customer/create_express_shipment");

                        request.Headers.Add("X-API-KEY", "JjEeEeBbLlYy1200");
                        request.Headers.Add("client_key", "967X250731093419Y4d6f7374616661536862616972");
                        request.Headers.Add("Cookie", "ci_session=dhas3ul30sh3jehcihes4rmeirgg6n2r; ci_session=4voddi7jac74917kgfcn30ksunkl0l7i");

                        request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                        var response = await _client.SendAsync(request);
                        //response.EnsureSuccessStatusCode();

                        var jsonString = await response.Content.ReadAsStringAsync();

                        // Convert to C# object
                        shipmentResponse = JsonSerializer.Deserialize<CreateOrderCourierResponse>(jsonString);
                        if (shipmentResponse.success == "true")
                        {
                            orderInfo.CourierOrderResponse = shipmentResponse.message;
                            orderInfo.CourierOrderId = shipmentResponse.AWB_No;
                            orderInfo.UpdateDate = Helper.DateTimeHelper.DubaiTime();
                        }
                        else
                        {
                            orderInfo.CourierOrderResponse = shipmentResponse.message;
                            orderInfo.UpdateDate = Helper.DateTimeHelper.DubaiTime();
                        }

                        await uow.OrderRequestProductRepository.Edit(orderInfo);
                    }
                  
                }
                return shipmentResponse;
            }
            catch (Exception ex)
            {
                var excep = new CreateOrderCourierResponse();
                excep.success = "error";
                excep.message = ex.Message;
                return excep;
            }
            
        }


        public async Task<bool> UpdateCourierOrderAsync(CourierOrderStatusResponse request)
        {
            try
            {
                var title = "";
                var message = "";
                // FIND ORDER FIRST
                var order = await uow.OrderRequestProductRepository.Get()
                    .FirstOrDefaultAsync(x => x.CourierOrderId == request.order_id);

                if (order == null)
                    return false;


                // FIND MATCHING ORDER STATUS (CourierStatusName & Active)
                var orderStatus = await uow.OrderStatusRepository.Get().FirstOrDefaultAsync(x =>
                    x.CourierStatusName == request.status &&
                    x.Status.ToLower() == "active");


                if (orderStatus != null)
                {
                    var orderstatus = new OrderMasterStatusLog();
                    orderstatus.OrderId = order.OrderRequestProductId;
                    orderstatus.CurrentInvoiceId = orderStatus.OrderStatusId;
                    orderstatus.PriviousInvoiceId = order.OrderStatusId.Value;
                    orderstatus.DateTime = request.event_date_time.AddHours(4);

                    await uow.OrderMasterStatusLogRepository.Add(orderstatus);

                    // If found -> update FK
                    if (orderStatus != null)
                    {
                        order.OrderStatusId = orderStatus.OrderStatusId;
                    }

                    // UPDATE RIDER INFO
                    order.RiderName = request.rider_name;
                    order.RiderPhone = request.rider_phone;
                    order.RiderPositionLat = request.lat;
                    order.RiderPositionLong = request.lng;
                    order.DeliveryImage = request.pod_image;

                    title = "Order Status Updated";
                    message = "Order No : #" + order.ReadableOrderId+" Status Condition Has been Updated by courier" ;
                    if (request.status == "Delivered")
                    {
                        if (order.PaymentMethodId == 1 && order.PaymentStatus == "Pending")
                        {
                            order.PaymentStatus = "Success";
                        }

                        title = "Order Delivered";
                        message = "Order No : #" + order.ReadableOrderId + " has been Delivered by courier";
                    }
                   

                    // Cancel logic
                    if (!string.IsNullOrEmpty(request.cancelreason))
                    {
                        order.IsCanceled = true;
                        order.CancelReason = request.cancelreason;

                        title = "Order canceled";
                        message = "Order No : #" + order.ReadableOrderId + " has been Canceled by Customer";
                    }

                    await uow.OrderRequestProductRepository.Edit(order);

                    await new AdminNotificationDataAccess().SaveNotification(Convert.ToInt32(order.MemberId), message, title, "Member", "Product", order.OrderRequestProductId);
                }

                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> CancelExpressShipmentAsync(int orderId)
        {
            try
            {
                var result = new CreateOrderCourierResponse();

                var orderInfo = await uow.OrderRequestProductRepository.Get().Where(s => s.OrderRequestProductId == orderId).Select(s => s.CourierOrderId).FirstOrDefaultAsync();
                if (!string.IsNullOrEmpty(orderInfo))
                {
                    var url = "https://demo.jeebly.com/customer/cancel_express_shipment";

                    var request = new HttpRequestMessage(HttpMethod.Post, url);

                    // Headers
                    request.Headers.Add("X-API-KEY", "JjEeEeBbLlYy1200");
                    request.Headers.Add("client_key", "967X250731093419Y4d6f7374616661536862616972");
                    request.Headers.Add("Cookie", "ci_session=vm49q01nrb0hfahah0o3tvtopah2n97o");

                    // Body
                    var body = new
                    {
                        order_id = orderId
                    };

                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(body);
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await _client.SendAsync(request);
                    var responseText = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {

                        return true;


                    }
                    else
                    {
                        return false;
                    }
                }
                return false;


                //return Newtonsoft.Json.JsonConvert.DeserializeObject<JeeblyCancelOrderResponse>(responseText);
            }
            catch (Exception ex)
            {

                return false;
            }
        }
    }


}