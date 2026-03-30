using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class MemberVehicalInfo : BaseEntity
    {
        public int MemberVehicalInfoId { get; set; }

        [ForeignKey(nameof(Brand))]
        public int BrandId { get; set; }

        public virtual Brand Brand { get; set; }

        [ForeignKey(nameof(VehicalModel))]
        public int VehicalModelId { get; set; }
        public virtual VehicalModel VehicalModel { get; set; }
        [StringLength(50)]
        public string Year { get; set; }
        [StringLength(500)]
        public string PlateNo { get; set; }

		[ForeignKey(nameof(Member))]
		public long MemberId { get; set; }
		public virtual Member Member { get; set; }




	}
}