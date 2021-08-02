using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWR_server
{
    public class ProductModel
    {
        public String url { get; set; }
        public String name { get; set; }
        public double price { get; set; }
        public String imgUrl { get; set; }
        public int id { get; set; }
        public int isOnSale { get; set; }
    }
}
