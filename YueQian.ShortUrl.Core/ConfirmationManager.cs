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

    public class ConfirmationManager
    {


        private const string characters = "123456789abcdefghijklmnopqrstuvwxyz";
        public System.Web.Security.MembershipUser CurrentUser;

        public ConfirmationManager() : this(null) { }

        public ConfirmationManager(string userName)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                var user = System.Web.Security.Membership.GetUser(userName);
                if (user != null)
                    CurrentUser = user;
            }
        }
        public string Generator(int length = 6)
        {
            var result = "";
            result = characters.Random(length);

            while (Exist(result))
            {
                result = characters.Random(length);
            }
            MongoHelper.Instance.Save(new ComfirmationCode
            {
                Code = result,
                CreationDate = DateTime.Now,
                IsConfirm = false,
                UserName = CurrentUser.UserName,
                UserId = CurrentUser.ProviderUserKey.ToString()
            });

            return result;
        }

        private bool Exist(string code)
        {
            var condition = new Dictionary<string, string>();
            condition.Add("Code", code);
            return MongoHelper.Instance.Has(condition, "ComfirmationCode");
        }

        /// <summary>
        /// 使用邀请码
        /// </summary>
        /// <param name="Code">邀请码</param>
        /// <param name="InvitedUserId">被邀请的人Id</param>
        /// <returns></returns>
        public bool Comfirm(string Code)
        {
            IMongoQuery condition = Query.EQ("Code", Code);
            var ic = MongoHelper.Instance.FindOne<ComfirmationCode>(condition);
            if (ic != null)
            {
                ic.IsConfirm = true;
                ic.ComfirmDate = DateTime.Now;

                if (MongoHelper.Instance.Save(ic))
                {
                    var usr = MongoHelper.Instance.FindOne<UserInfo>(Query.EQ("UserId", ic.UserId));
                    usr.IsComfirm = true;
                    CurrentUser = System.Web.Security.Membership.GetUser(ic.UserName);
                    return MongoHelper.Instance.Save(usr);
                }
            }
            return false;
        }
    }
}
