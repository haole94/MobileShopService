using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MobileShopService.Models
{
    public class ProductModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Nullable<int> ID_Manufacturer { get; set; }
        public Nullable<int> Price { get; set; }
        public string Screen { get; set; }
        public string PrimaryCamera { get; set; }
        public string SecondaryCamera { get; set; }
        public string OS { get; set; }
        public string CPU { get; set; }
        public string GraphicChip { get; set; }
        public string RAM { get; set; }
        public string Storage { get; set; }
        public string SD { get; set; }
        public string SIM { get; set; }
        public string Battery { get; set; }
        public string Comms { get; set; }
        public byte[] Image { get; set; }
        public Nullable<int> IsVisible { get; set; }
    }
}