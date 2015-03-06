using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

namespace YueQian.ShortUrl.Core
{

    public class RequestUser
    {
        public MembershipUser CurrentUser { get; private set; }

        public RequestUser()
        {
            CurrentUser = Membership.GetUser(System.Web.HttpContext.Current.User.Identity.Name);
            if (CurrentUser == null) throw new NotImplementedException("请重新登录");
        }

    }
}
