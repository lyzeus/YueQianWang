using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using YueQian.ShortUrl.Models;

namespace YueQian.ShortUrl.Core
{
    public class IntegralCalculator
    {
        /// <summary>
        /// 可用积分
        /// </summary>
        public int Integral { get; private set; }

        /// <summary>
        /// 全部积分
        /// </summary>
        public int TotalIntegral { get; private set; }

        /// <summary>
        /// 已使用积分
        /// </summary>
        public int UsedIntegral { get; private set; }

        /// <summary>
        /// 冻结的积分
        /// </summary>
        public int FrozenIntegral { get; private set; }

        public IntegralCalculator(string userid)
        {
            IMongoQuery condition = Query.EQ("UserId", userid);
            condition = Query.And(condition, Query.EQ("IsDelete", false));

            var totalSource = MongoHelper.Instance.Find<Integral>(condition);
            TotalIntegral = totalSource.Sum(s => s.Result);

            var usedSource = MongoHelper.Instance.Find<IntegralUsed>(condition);
            UsedIntegral = usedSource.Sum(s => s.Integral);

            var frozenSource = MongoHelper.Instance.Find<IntegralFrozen>(condition);
            FrozenIntegral = frozenSource.Sum(s => s.Integral);

            Integral = TotalIntegral - UsedIntegral - FrozenIntegral;
        }
    }
}
