//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MobileShopService
{
    using System;
    using System.Collections.Generic;
    
    public partial class Account
    {
        public Account()
        {
            this.Carts = new HashSet<Cart>();
        }
    
        public string Username { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public string Name { get; set; }
        public string Birthday { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
    
        public virtual ICollection<Cart> Carts { get; set; }
    }
}
