using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class Member : BaseEntity
    {
        public Member()
        {
            this.MemberAddresses = new List<MemberAddress>();
        }
        [Key]
        public long MemberId { get; set; }
     
        public Guid MemberKey { get; set; }
        [StringLength(150)]
        public string Name { get; set; }
  
        [StringLength(200)]
        public string Email { get; set; }
        [StringLength(50)]
        public string PhoneNumber { get; set; }

        [StringLength(50)]
        public string PhoneCode { get; set; }

        [StringLength(150)]
        public string Password { get; set; }
       
        public string Image { get; set; }
        [StringLength(250)]
        public string Address { get; set; }
      
        [StringLength(150)]
        public string SecurityToken { get; set; }
       


        public string ThirdPartyKey { get; set; }


        [StringLength(50)]
        public string OTPNumber { get; set; }

        public DateTime? OTPGenerateDateTime { get; set; }

        public int MonthlyGoalId { get; set; }
        public decimal? MonthlyGoalAmount{ get; set; }

        public bool ThirdPartyLogin { get; set; }

  
        public string ThirdPartyLoginKey { get; set; }

        [StringLength(250)]
        public string ThirdPartyLoginFrom { get; set; }
        [NotMapped]
        public double? MonthlyGoalAchivedAmount { get; set; }



        [NotMapped]
        public bool IsAnyMembership { get; set; }

        [NotMapped]
        public MemberSubscription MemberSubscriptions { get; set; }


        [NotMapped]
        public List<MemberAddress> MemberAddresses { get; set; }

        [NotMapped]
        public string MemberShipName { get; set; }
    }
}