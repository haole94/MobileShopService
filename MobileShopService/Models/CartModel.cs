using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MobileShopService.Models
{
    public class CartModel
    {
        public int ID { get; set; }
        public string PurchasedDate { get; set; }
        public string Receiver { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public Nullable<int> Checked { get; set; }
        public Nullable<int> ConfirmedByAdmin { get; set; }
        public string Username { get; set; }
    }
}