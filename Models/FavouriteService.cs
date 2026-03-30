using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
	public class FavouriteService
	{
		public int FavouriteServiceId { get; set; }

		
		public int? ServiceId { get; set; }
	

		
		public int ServiceTypeId { get; set; }


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