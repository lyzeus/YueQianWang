using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMatrix.WebData;
using YueQian.ShortUrl.Models;
using YueQian.ShortUrl.Web.Models;
using ExtendedMongoMembership;
using System.Web.Security;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace YueQian.ShortUrl.Web.Helpers
{
    public class UserHelper
    {
        public static bool Exists(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return false;
            return WebSecurity.UserExists(userName);
        }

        public static bool Register(UserRegisterModel usr)
        {
            var usrKey = WebSecurity.CreateUserAndAccount(usr.UserName, usr.Password, requireConfirmationToken: true);

            if (!string.IsNullOrEmpty(usrKey))
            {
                Roles.AddUserToRole(usr.UserName, "User");
                WebSecurity.ConfirmAccount(usrKey);

                var user = new UserInfo
                {
                    CreationDate = DateTime.Now,
                    Email = usr.Email,
                    IsDelete = false,
                    UserId = Membership.GetUser(usr.UserName).ProviderUserKey.ToString()
                };
                if (!MongoHelper.Instance.Save(user))
                {
                    new MongoMembershipProvider().DeleteUser(usr.UserName, true);
                    return false;
                }
                WebSecurity.Login(usr.UserName, usr.Password);
                return true;
            }
            return false;

        }

        public static Verification GetVerification(string userid)
        {
            IMongoQuery condition = Query.EQ("UserId", userid);
            condition = Query.And(condition, Query.EQ("IsDelete", false));

            var model = MongoHelper.Instance.FindOne<Verification>(condition);
            return model;
        }
    }
}