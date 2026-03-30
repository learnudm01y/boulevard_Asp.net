using Boulevard.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Boulevard.ResponseModel
{
    public class OrderRequestProductResponse
    {
        public OrderRequestProductResponse()
        {
            OrderRequestProduct = new List<OrderRequestProductResponse>();
        }
        public int OrderRequestProductId { get; set; }
        public string ReadableOrderId { get; set; }

        public long MemberId { get; set; }
        public virtual Member Member { get; set; }

        public long? MemberAddressId { get; set; }

        public virtual MemberAddress MemberAddresses { get; set; }
        public DateTime OrderDateTime { get; set; }

        public DateTime DeliveryDateTime { get; set; }

        public string Comments { get; set; }

        public double DeliveryCharge { get; set; }
        public double TotalPrice { get; set; }

        public int PaymentMethodId { get; set; }

        public virtual PaymentMethod PaymentMethod { get; set; }

        public int? OrderStatusId { get; set; }

        public virtual OrderStatus OrderStatus { get; set; }
        public List<ProductSmallDetailsResponse> OrderRequestDetails { get; set; }

        public List<OrderRequestProductDetails> OrderRequestProduxtDetails { get; set; }
        public string OrderStatusName { get; set; }
        public List<OrderRequestProductResponse> OrderRequestProduct { get; set; }
    }
}