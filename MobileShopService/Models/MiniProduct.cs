using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MobileShopService.Models
{
    public class MiniProduct
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Nullable<int> ID_Manufacturer { get; set; }
        public Nullable<int> Price { get; set; }
        public byte[] Image { get; set; }
    }
}