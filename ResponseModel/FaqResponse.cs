using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boulevard.ResponseModel
{
	public class FaqResponse
	{
		public int FaqServiceId { get; set; }

		public string FaqTitle { get; set; }

		public string FaqDescription { get; set; }
	}
}