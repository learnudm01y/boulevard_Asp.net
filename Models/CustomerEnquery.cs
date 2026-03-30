using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class CustomerEnquery
    {
        [Key]
        public int CustomerEnqueryId { get; set; }
        [StringLength(250)]
        public string Name { get; set; }
        [StringLength(250)]
        public string Email { get; set; }

        public string Message { get; set; }

        [StringLength(50)]
        public string PhoneCode { get; set; }

        [StringLength(100)]
        public string Phonenumber { get; set; }

        [ForeignKey(nameof(FeatureCategory))]
        public int FeatureCategoryId { get; set; }
        public virtual FeatureCategory FeatureCategory { get; set; }

        public DateTime UpdatedAt { get; set; }


        public int UserId { get; set; }

        [StringLength(100)]
        public string Status { get; set; }

        [NotMapped]
        public string FeatureCategoryName { get; set; }
        [NotMapped]
        public string UserName { get; set; }
        [NotMapped]
        public string UserEmail { get; set; }
        [NotMapped]
        public string UserPhoneNo { get; set; }




    }
}