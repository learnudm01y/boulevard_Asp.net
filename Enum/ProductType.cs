using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Boulevard.Enum
{
    public enum ProductType
    {
        [Display(Name = "now", Description = "الآن")]
        Now = 1,

        [Display(Name = "scheduled", Description = "مجدول")]
        Scheduled = 2,

        [Display(Name = "now and scheduled", Description = "الآن ومجدول")]
        NowAndScheduled = 3
    }
}