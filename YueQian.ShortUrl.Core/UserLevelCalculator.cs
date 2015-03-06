using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YueQian.ShortUrl.Models;

namespace YueQian.ShortUrl.Core
{


    public class UserLevelCalculator
    {
        public MemberShipLevel Level;

        public UserLevelCalculator(int integral)
        {
            this.Level = GetLevel(integral);
        }

        public UserLevelCalculator(string userId)
        {
            var calc = new IntegralCalculator(userId);
            this.Level = GetLevel(calc.Integral);
        }

        private MemberShipLevel GetLevel(int integral)
        {
            var levels = MongoHelper.Instance.Find<MemberShipLevel>(Query.Null);

            Func<MemberShipLevel, bool> condition = m => ((integral < m.MaxIntegral) && (integral >= m.MinIntegral));

            MemberShipLevel level = levels.FirstOrDefault(condition);
            if (level == null)
                level = levels.OrderBy(m => m.MinIntegral).FirstOrDefault();
            if (level == null)
                level = new MemberShipLevel { Name = "未定义" };
            return level;
        }


    }
}
