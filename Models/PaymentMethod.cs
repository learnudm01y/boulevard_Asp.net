using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class PaymentMethod :BaseEntity
    {
        public int PaymentMethodId { get; set; }

        public Guid PaymentMethodKey { get; set; }

        public string PaymentMethodName { get; set;}
        public string PaymentMethodNameAr { get; set; }

    }
}