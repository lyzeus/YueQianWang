using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YueQian.ShortUrl.Extensions;
using YueQian.ShortUrl.Models;
using YueQian.ShortUrl.Models.Enums;

namespace YueQian.ShortUrl.Core
{

    public class InviteCodeManager
    {

        private readonly int MaxInvite = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MaxInvite"]);


        private const string characters = "123456789abcdefghijklmnopqrstuvwxyz";
        private System.Web.Security.MembershipUser CurrentUser = System.Web.Security.Membership.GetUser(System.Web.HttpContext.Current.User.Identity.Name);

        public InviteCodeManager() : this(null) { }

        public InviteCodeManager(string userName)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                var user = System.Web.Security.Membership.GetUser(userName);
                if (user != null)
                    CurrentUser = user;
            }
        }
        private string Generator(int length = 6)
        {
            var result = "";
            result = characters.Random(length);

            while (Exist(result))
            {
                result = characters.Random(length);
            }
            MongoHelper.Instance.Save(new InviteCode()
            {
                Code = result,
                CreationDate = DateTime.Now,
                IsUsed = false,
                UserId = CurrentUser.ProviderUserKey.ToString()
            });

            return result;
        }

        private bool Exist(string code)
        {
            var condition = new Dictionary<string, string>();
            condition.Add("Code", code);
            return MongoHelper.Instance.Has(condition, "InviteCode");
        }

        public IEnumerable<InviteCode> GetInviteCodeList(bool? used)
        {
            IMongoQuery condition = Query.EQ("UserId", CurrentUser.ProviderUserKey.ToString());
            if (used.HasValue)
                condition = Query.And(condition, Query.EQ("IsUsed", used.Value));
            var source = MongoHelper.Instance.Find<InviteCode>(condition);
            return source;
        }

        public string GenerateCode(bool alwaysNew = false)
        {
            if (alwaysNew) return Generator();
            IMongoQuery condition = Query.EQ("UserId", CurrentUser.ProviderUserKey.ToString());
            var ics = MongoHelper.Instance.Find<InviteCode>(condition);

            //未邀请过
            if (ics == null || ics.Count() <= 0) return Generator();

            var usedSourceCount = ics.Count(s => s.IsUsed);
            //邀请到达最大值
            if (usedSourceCount >= MaxInvite) return "";

            //存在未使用的邀请码
            var notUsedSource = ics.Where(s => !s.IsUsed);
            if (notUsedSource != null && notUsedSource.Count() > 0)
                return notUsedSource.FirstOrDefault().Code;

            return Generator();
        }

        /// <summary>
        /// 使用邀请码
        /// </summary>
        /// <param name="Code">邀请码</param>
        /// <param name="InvitedUserId">被邀请的人Id</param>
        /// <returns></returns>
        public bool UseInviteCode(string Code, string InvitedUserId, string InvitedUserName)
        {
            IMongoQuery condition = Query.EQ("Code", Code);
            var ic = MongoHelper.Instance.FindOne<InviteCode>(condition);
            if (ic != null)
            {
                ic.IsUsed = true;
                ic.InvitedUserId = InvitedUserId;
                ic.InvitedUserName = InvitedUserName;
                ic.UsedDate = DateTime.Now;


                if (MongoHelper.Instance.Save(ic))
                {
                    #region /*(-_-)*/为邀请人增加积分/*(-_-)*/
                    var integral = new Integral
                    {
                        UserId = ic.UserId,
                        CalculatorSystem = Application.CalculatorSystem,
                        IntegralType = IntegralType.用户推广,
                        CreationDate = DateTime.Now
                    };
                    return MongoHelper.Instance.Save(integral);
                    #endregion
                }
                return false;



            }
            return false;
        }


        public bool Valid(string invite)
        {
            IMongoQuery condition = Query.EQ("IsUsed", false);
            condition = Query.And(condition, Query.EQ("Code", invite));
            var ics = MongoHelper.Instance.Count<InviteCode>(condition);
            return ics > 0;

        }
    }
}
