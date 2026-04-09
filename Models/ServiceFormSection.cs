using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Boulevard.Models
{
    [Table("ServiceFormSections")]
    public class ServiceFormSection
    {
        [Key]
        public int SectionId { get; set; }

        [ForeignKey(nameof(ServiceType))]
        public int ServiceTypeId { get; set; }
        public virtual ServiceType ServiceType { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; }

        [StringLength(200)]
        public string TitleAr { get; set; }

        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }

        public int CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }

        public virtual ICollection<ServiceFormField> Fields { get; set; }
    }
}
