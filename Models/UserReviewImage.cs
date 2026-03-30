using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class UserReviewImage
    {
        public int UserReviewImageId { get; set; }

        [ForeignKey(nameof(UserReview))]
        public int UserReviewId { get; set; }
        public virtual UserReview UserReview { get; set; }
        [StringLength(180)]
        public string Image { get; set; }
    }
}