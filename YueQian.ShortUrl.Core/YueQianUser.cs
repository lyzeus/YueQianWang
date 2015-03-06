using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YueQian.ShortUrl.Models;

namespace YueQian.ShortUrl.Core
{
    public class YueQianUser
    {
        private string userId;

        public YueQianUser(string userId)
        {
            this.userId = userId;
        }

        public UserInfo UserInfo
        {
            get
            {
                return MongoHelper.Instance.FindOne<UserInfo>(MongoDB.Driver.Builders.Query.EQ("UserId", userId));
            }
        }

        public IntegralCalculator IntegralCalculator { get { return new IntegralCalculator(userId); } }

        public UserLevelCalculator UserLevelCalculator { get { return new UserLevelCalculator(userId); } }

    }
}
