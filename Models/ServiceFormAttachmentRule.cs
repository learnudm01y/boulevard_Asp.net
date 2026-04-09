using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Boulevard.Models
{
    [Table("ServiceFormAttachmentRules")]
    public class ServiceFormAttachmentRule
    {
        [Key]
        public int AttachmentRuleId { get; set; }

        [ForeignKey(nameof(Field))]
        public int FieldId { get; set; }
        public virtual ServiceFormField Field { get; set; }

        [Required, StringLength(500)]
        public string AllowedExtensions { get; set; }

        public int MaxFileSizeMB { get; set; }
        public bool AllowMultiple { get; set; }
        public bool IsRequired { get; set; }
        public int MaxFileCount { get; set; }

        [StringLength(200)]
        public string DisplayLabel { get; set; }

        [StringLength(200)]
        public string DisplayLabelAr { get; set; }
    }
}
