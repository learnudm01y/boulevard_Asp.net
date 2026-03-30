using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class OrderMasterStatusLog
    {
        public int OrderMasterStatusLogId { get; set; }

        public int OrderId { get; set; }
        public int CurrentInvoiceId { get; set; }

        public int PriviousInvoiceId { get; set; }
        public DateTime DateTime { get; set; }

        public int CreatedBy { get; set; }
    }
}