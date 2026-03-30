using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class MemberAddress : BaseEntity
    {
        public long MemberAddressId { get; set; }
        [JsonIgnore]
        public Guid MemberAddressKey { get; set; }

         public string Type { get; set; }
        public int CountryId { get; set; }
        public int CityId { get; set; }
        [StringLength(250)]
        public string AddressLine1 { get; set; }
        [StringLength(250)]
        public string AddressLine2 { get; set; }
        [StringLength(250)]
        public string NearByAddress { get; set; }
        [ForeignKey(nameof(Member))]
        public long MemberId { get; set; }
        [JsonIgnore]
        public virtual Member Member { get; set; }

        [StringLength(250)]
        public string longitude { get; set; }
        [StringLength(250)]
        public string latitude { get; set; }
        public bool IsDefault { get; set; }
        [NotMapped]
        public virtual Country Country { get; set; }
        [NotMapped]
        public virtual City City { get; set; }  

    }
}