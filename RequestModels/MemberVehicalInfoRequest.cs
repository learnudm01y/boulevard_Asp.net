using Boulevard.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Boulevard.RequestModels
{
	public class MemberVehicalInfoRequest
	{
		public int MemberVehicalInfoId { get; set; }

		
		public int BrandId { get; set; }

		

		public int VehicalModelId { get; set; }


		public string Year { get; set; }

		public string PlateNo { get; set; }

	
		public long MemberId { get; set; }




	}
}