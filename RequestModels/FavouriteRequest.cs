using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boulevard.RequestModels
{
	public class FavouriteRequest
	{
		public int ProductId { get; set; }

		public int MemberId { get; set; }

		public int ServiceId { get; set; }

		public int ServiceTypeId { get; set; }

		public int FeatureCategoryId { get; set; }
	}
}