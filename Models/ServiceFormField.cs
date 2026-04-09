using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Boulevard.Models
{
    [Table("ServiceFormFields")]
    public class ServiceFormField
    {
        [Key]
        public int FieldId { get; set; }

        [ForeignKey(nameof(Section))]
        public int SectionId { get; set; }
        public virtual ServiceFormSection Section { get; set; }

        [Required, StringLength(100)]
        public string FieldKey { get; set; }

        [Required, StringLength(200)]
        public string Label { get; set; }

        [StringLength(200)]
        public string LabelAr { get; set; }

        [StringLength(300)]
        public string Placeholder { get; set; }

        [StringLength(300)]
        public string PlaceholderAr { get; set; }

        [Required, StringLength(50)]
        public string FieldType { get; set; }   // text, number, dropdown, radio, checkbox, file, etc.

        [Required, StringLength(50)]
        public string DataType { get; set; }    // string, int, decimal, bool, date, time, file

        public bool IsRequired { get; set; }
        public bool IsVisible { get; set; }
        public int SortOrder { get; set; }

        [StringLength(500)]
        public string DefaultValue { get; set; }

        [StringLength(500)]
        public string HelpText { get; set; }

        [StringLength(500)]
        public string HelpTextAr { get; set; }

        [StringLength(500)]
        public string ValidationRegex { get; set; }

        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }

        [StringLength(100)]
        public string MinValue { get; set; }

        [StringLength(100)]
        public string MaxValue { get; set; }

        public bool IsDelete { get; set; }
        public int CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }

        public virtual ICollection<ServiceFormFieldOption> Options { get; set; }
        public virtual ICollection<ServiceFormAttachmentRule> AttachmentRules { get; set; }
    }
}
