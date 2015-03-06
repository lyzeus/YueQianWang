using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace YueQian.ShortUrl.Web
{
    public class SiteConfig
    {
        public static void RegisterRoles()
        {
            var roles = Roles.GetAllRoles();
            if (!roles.Contains("Admin"))
                Roles.CreateRole("Admin");
            if (!roles.Contains("User"))
                Roles.CreateRole("User");
        }
    }
}