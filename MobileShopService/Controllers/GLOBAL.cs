using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MobileShopService.Controllers
{
    public class GLOBAL
    {
        public static bool CHECK_TOKEN(string token)
        {
            DienTuEntities ctx = new DienTuEntities();
            List<Account> list = ctx.Accounts.ToList();

            Account acc = list.Where(t => t.Token == token).FirstOrDefault();

            if (acc == null)
                return false;

            return true;
        }
    }
}