using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class CommonProductTagDetails
    {
        public int CommonProductTagDetailsId { get; set; }

        public int ProductId { get; set; }

        public int CommonProductTagId { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}