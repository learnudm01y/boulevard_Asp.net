using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;
using Boulevard.ResponseModel;

namespace Boulevard.Models
{
    public class OrderRequestProduct:BaseEntity
    {
        public OrderRequestProduct() {
            OrderRequestDetails = new List<ProductSmallDetailsResponse>();
            OrderRequestProduxtDetails = new List<OrderRequestProductDetails>();
        }
        public int OrderRequestProductId { get; set; }
        public string ReadableOrderId { get; set; }

        [ForeignKey("Member")]
        public long MemberId { get; set; }
        public virtual Member Member { get; set; }

        [ForeignKey("MemberAddresses")]
        public long? MemberAddressId { get; set; }

        public virtual MemberAddress MemberAddresses { get; set; }
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime OrderDateTime { get; set; }
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public DateTime DeliveryDateTime { get; set; }

        [StringLength(500)]
        public string Comments { get; set; }

        public double DeliveryCharge { get; set; }
        public double TotalPrice { get; set; }

        public double ServiceCharge { get; set; }

        public double Tip { get; set; }

        [ForeignKey("PaymentMethod")]
        public int PaymentMethodId { get; set; }

        public virtual PaymentMethod PaymentMethod { get; set; }

        [ForeignKey("OrderStatus")]
        public int? OrderStatusId { get; set; }

        public virtual OrderStatus OrderStatus { get; set; }


        [ForeignKey(nameof(FeatureCategory))]
        public int? FeatureCategoryId { get; set; }
        public virtual FeatureCategory FeatureCategory { get; set; }

        [StringLength(100)]
        public string PaymentStatus { get; set; }

        [StringLength(100)]
        public string PaymentTransectionId { get; set; }
        public bool IsSound { get; set; } = false;
        public bool IsAdmin { get; set; } = false;

        public int ProductType { get; set; }

        [StringLength(100)]
        public string CourierOrderId { get; set; }

        [StringLength(500)]
        public string CourierOrderResponse { get; set; }

        [StringLength(100)]
        public string RiderName { get; set; }
        [StringLength(100)]
        public string RiderPhone{ get; set; }

        [StringLength(100)]
        public string RiderPositionLat { get; set; }

        [StringLength(100)]
        public string RiderPositionLong { get; set; }

        [StringLength(500)]
        public string DeliveryImage { get; set; }

       
        public bool IsCanceled { get; set; }


        public string CancelReason { get; set; }


        [NotMapped]

        public string ProductTypeName { get; set; }


        [NotMapped]

        public List<OrderStatus> OrderStatushistory { get; set; }

        [NotMapped]
        public List<ProductSmallDetailsResponse> OrderRequestDetails { get; set; }
        
        [NotMapped]
        public List<OrderRequestProductDetails> OrderRequestProduxtDetails { get; set; }
        [NotMapped]
        public string OrderStatusName { get; set; }
        [NotMapped]
        public int CountryId { get; set; }
        [NotMapped]
        public int CityId { get; set; }



    }
}