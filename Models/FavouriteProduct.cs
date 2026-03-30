using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
	public class FavouriteProduct
	{
		public int FavouriteProductId { get; set; }

		[ForeignKey("Product")]
		public int ProductId { get; set; }
		public virtual Product Product { get; set; }

		[ForeignKey(nameof(Member))]
		public long MemberId { get; set; }
		public virtual Member Member { get; set; }

		[ForeignKey(nameof(FeatureCategory))]
		public int FeatureCategoryId { get; set; }
		public virtual FeatureCategory FeatureCategory { get; set; }

		public bool Status { get; set; }
		public DateTime LastModified { get; set; }


	}
}