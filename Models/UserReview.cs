using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class UserReview :BaseEntity
    {
        public UserReview() {
            UserReviewImages = new List<UserReviewImage>();
        }
        public int UserReviewId { get; set; }

        public int UserId { get; set; }

        public string UserType { get; set; }

        public string FeatureType { get; set; }

        public int FeatureTypeId { get; set; }

        public int Rating { get; set; }
        public string Comment { get; set; }

        public string Details { get; set; }

        public List<UserReviewImage> UserReviewImages { get; set; }

        [NotMapped]
        public string MemberName{ get; set; }




    }
}