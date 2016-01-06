using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MobileShopService.Models
{
    public class ItemModel
    {
        public int ID_Product { get; set; }
        public int ID_Cart { get; set; }
        public string Name { get; set; }
        public byte[] Image { get; set; }
        public Nullable<int> Price { get; set; }
        public Nullable<int> Quantity { get; set; }
    }
}