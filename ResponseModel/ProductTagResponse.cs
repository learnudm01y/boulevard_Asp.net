using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Boulevard.ResponseModel
{
    public class ProductTagResponse
    {
        public int CommonProductTagId { get; set; }

    
        public string TagName { get; set; }
    }
}