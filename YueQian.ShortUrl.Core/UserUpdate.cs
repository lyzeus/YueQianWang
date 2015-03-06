using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YueQian.ShortUrl.Models;

namespace YueQian.ShortUrl.Core
{
    public class UserUpdate : RequestUser
    {
        private UserUpdate() { }
        public string UserId { get; set; }
        public UserUpdate(string userId)
        {
            this.UserId = userId;
        }

        public bool UpdateUserLevel()
        {
            var calculator = new IntegralCalculator(UserId);
            var integral = calculator.Integral;


            var um = new UserLevelManager(UserId);
            var level = new UserLevelCalculator(integral).Level;
            var ul = um.UserLevel;
            var currentLevel = um.Level;


            if (currentLevel == null || !currentLevel.Equals(level))
            {
                if (ul == null) ul = new UserLevel();
                ul.Level = level;
                if (ul.UserLevelRecord == null)
                    ul.UserLevelRecord = new List<UserLevelRecord>();
                ul.UserLevelRecord.Add(new UserLevelRecord
                {
                    CreationDate = DateTime.Now,
                    CurrentPoint = integral,
                    IsDelete = false,
                    Level = level,
                    ManagerId = CurrentUser.ProviderUserKey.ToString(),
                    ManagerName = CurrentUser.UserName
                });

                return MongoHelper.Instance.Save(ul);
            }


            return false;
        }
    }
}
