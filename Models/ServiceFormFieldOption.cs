using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Boulevard.Models
{
    [Table("ServiceFormFieldOptions")]
    public class ServiceFormFieldOption
    {
        [Key]
        public int OptionId { get; set; }

        [ForeignKey(nameof(Field))]
        public int FieldId { get; set; }
        public virtual ServiceFormField Field { get; set; }

        [Required, StringLength(200)]
        public string OptionLabel { get; set; }

        [StringLength(200)]
        public string OptionLabelAr { get; set; }

        [Required, StringLength(200)]
        public string OptionValue { get; set; }

        public int SortOrder { get; set; }
        public bool IsDefault { get; set; }
        public bool IsDelete { get; set; }
    }
}
