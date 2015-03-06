using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace YueQian.ShortUrl.ViewModels
{
    public class ViewModelBase
    {

        private string _Title;
        public string Title
        {
            get { return string.Format("{0} - 跃迁网", _Title); }
            protected set { _Title = value; }
        }

        public string PageTitle { get; set; }
        public string SubTitle { get; set; }


        public virtual string Keyword
        {
            get { return "常房网,常州房产网,常州房地产,运动休闲部落,常州运动休闲"; }
        }

        public virtual string Descrption
        {
            get { return "常房网是专业常州房产网站，常州最大的房地产家居网络平台，提供全面及时的常州房产新闻资讯内容为常州房产楼盘提供网上浏览、常房网努力做全国最大影响力的房产综合门户"; }
        }

        protected MembershipUser yqUserIdentity
        {
            get
            {
                if (!HttpContext.Current.Request.IsAuthenticated)
                    throw new NotImplementedException("请重新登录");
                return System.Web.Security.Membership.GetUser(HttpContext.Current.User.Identity.Name);
            }
        }

        protected string CurrentUserId
        {
            get
            {
                return yqUserIdentity.ProviderUserKey.ToString();
            }
        }
    }
}