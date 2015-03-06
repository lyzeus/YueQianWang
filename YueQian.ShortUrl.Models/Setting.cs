using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YueQian.ShortUrl.Models
{
    public class Setting : LongIdEntity
    {
        /// <summary>
        /// 积分系统
        /// </summary>
        public int CalculatorSystem { get; set; }

        public IEnumerable<string> CompanyAccounts
        {
            get
            {
                var source = MongoHelper.Instance.Find<CompanyAccount>(MongoDB.Driver.Builders.Query.Null);
                foreach (var item in source)
                {
                    yield return item.FullName;
                }
            }
        }
    }

}
