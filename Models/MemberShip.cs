using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Boulevard.Models
{
    public class MemberShip:BaseEntity
    {
        public MemberShip()
        {
            DiscountInfo = new List<MemberShipDiscountCategory>();
        }
        public int MemberShipId { get; set; }

        public Guid MemberShipKey { get; set; }

        [StringLength(250)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public int MembershipValidityInMonth { get; set; }


        [AllowHtml]
        public string Benefits { get; set; }

        [StringLength(250)]

        public string MembershipBanner { get; set; }

        [NotMapped]
        public List<MemberShipDiscountCategory> DiscountInfo { get; set; }
        [StringLength(250)]
        public string TitleAr { get; set; }

        [StringLength(500)]
        public string DescriptionAr { get; set; }
        [AllowHtml]
        public string BenefitsAr { get; set; }

        [StringLength(250)]

        public string MembershipBannerAr { get; set; }

    }
}