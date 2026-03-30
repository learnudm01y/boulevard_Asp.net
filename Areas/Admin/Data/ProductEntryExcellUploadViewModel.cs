using Boulevard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boulevard.Areas.Admin.Data
{
    public class ProductEntryExcellUploadViewModel
    {
        public string SrNo { get; set; }
        public string Brand { get; set; }
        public string Barcode { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string ItemDesc { get; set; }
        public string AttributeCode { get; set; }
        public string AttributeName { get; set; }
        public string Images { get; set; }    
        public string Quantity { get; set; }
        public string SellingPrice { get; set; }
        public string SecondQuantity { get; set; }
        public string SubSkuId { get; set; }
        public string ProductTags { get; set; }
        public string StocksQuantity { get; set; }

        public List<string> ImageUrls = new List<string>();
        public List<ProductPrice> productPrices = new List<ProductPrice>();
    }
}
