using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YueQian.ShortUrl.Models;

namespace YueQian.ShortUrl.Core
{
    public class UserLevelManager
    {
        /// <summary>
        /// 用户等级信息
        /// </summary>
        public UserLevel UserLevel { get; set; }

        public string UserId { get; set; }

        /// <summary>
        /// 用户等级
        /// </summary>
        public MemberShipLevel Level { get; set; }

        public UserLevelManager(string userId)
        {
            var condition = Query.EQ("UserId", userId);
            var userLevel = MongoHelper.Instance.FindOne<UserLevel>(condition);
            if (userLevel != null)
            {
                this.UserLevel = userLevel;
                this.Level = userLevel.Level;
            }
            else
            {
                var currentPoint = new IntegralCalculator(userId).Integral;
                var level = new UserLevelCalculator(currentPoint).Level;

                userLevel = new UserLevel
                {
                    CreationDate = DateTime.Now,
                    Level = level,
                    UserId = userId,
                    UserLevelRecord = new List<UserLevelRecord>() {
                        new UserLevelRecord{
                            CreationDate=DateTime.Now,
                            CurrentPoint=currentPoint,
                            Level=level,
                            UserId=userId
                        }
                    }
                };
                MongoHelper.Instance.Save(userLevel);

                this.UserLevel = userLevel;
                this.Level = userLevel.Level;
            }

        }


        public string GetLevelName()
        {
            if (Level != null)
                return Level.Name;
            return "";
        }

    }
}
