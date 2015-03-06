using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace YueQian.ShortUrl.Web.Controllers
{
    public class BaseController : Controller
    {
        public System.Web.Security.MembershipUser yqUserIdentity
        {
            get
            {
                if (!Request.IsAuthenticated)
                    throw new NotImplementedException("请重新登录");
                return System.Web.Security.Membership.GetUser(User.Identity.Name);
            }
        }

        public string CurrentUserId
        {
            get
            {
                return yqUserIdentity.ProviderUserKey.ToString();
            }
        }

        public IMongoQuery UserIdentityQueryBuilder(IMongoQuery query)
        {
            var condition = Query.EQ("UserId", CurrentUserId);
            if (query != null && query != Query.Null)
                condition = Query.And(condition, query);
            return condition;
        }

        public string CurrentVerifyCode
        {
            get
            {
                var code = Session["vcCode"];
                if (code != null && !string.IsNullOrEmpty(code.ToString()))
                    return code.ToString();
                return "";
            }
        }
    }
}
