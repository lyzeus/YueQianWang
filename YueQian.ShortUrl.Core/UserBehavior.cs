using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using MongoDB.Driver.Builders;
using YueQian.ShortUrl.Models;
using YueQian.ShortUrl.Models.Enums;

namespace YueQian.ShortUrl.Core
{

    public class UserBehavior
    {
        public MembershipUser yqUserIdentity;

        public string CurrentUserId;

        public static string GetUserIdByUserName(string username)
        {
            var user = System.Web.Security.Membership.GetUser(username);
            if (user != null)
                return user.ProviderUserKey.ToString();
            return "";
        }
        public UserBehavior()
        {
            if (!HttpContext.Current.Request.IsAuthenticated)
                throw new NotImplementedException("请重新登录");
            yqUserIdentity = System.Web.Security.Membership.GetUser(HttpContext.Current.User.Identity.Name);
            CurrentUserId = yqUserIdentity.ProviderUserKey.ToString();
        }

        /// <summary>
        /// 用户签到
        /// </summary
        /// Item1签到结果,Item2提示信息
        /// <returns></returns>
        public Tuple<bool, string> Sign()
        {
            if (!HttpContext.Current.Request.IsAuthenticated)
                return new Tuple<bool, string>(false, "签到失败,请尝试刷新本页或者请重新登录!");

            if (IsUserSign(DateTime.Now))
                return new Tuple<bool, string>(false, "您今天已经签过了!");

            var model = new UserSignIn
            {
                CreationDate = DateTime.Now,
                UserId = CurrentUserId
            };
            var result = MongoHelper.Instance.Save(model);
            var msg = "签到失败!";
            if (result)
            {
                var integral = new Integral
                {
                    UserId = CurrentUserId,
                    CalculatorSystem = Application.CalculatorSystem,
                    IntegralType = IntegralType.签到,
                    CreationDate = DateTime.Now
                };
                MongoHelper.Instance.Save(integral);
                msg = string.Format("{0} 签到成功!", DateTime.Now.ToString("yyyy/MM/dd"));
            }
            return new Tuple<bool, string>(result, msg);
        }

        /// <summary>
        /// 判断当前用户指定日期是否签到
        /// </summary>
        /// <param name="datetime">指定日期</param>
        /// <returns></returns>
        public bool IsUserSign(DateTime? datetime)
        {
            datetime = datetime ?? DateTime.Now;
            var condition = Query.EQ("UserId", CurrentUserId);
            condition = Query.And(condition, Query.GTE("CreationDate", datetime.Value.Date));
            condition = Query.And(condition, Query.LT("CreationDate", datetime.Value.Date.AddDays(1)));
            var count = MongoHelper.Instance.Count<UserSignIn>(condition);
            return count > 0;
        }
    }
}
