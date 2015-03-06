using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using YueQian.ShortUrl.Models.Enums;

namespace YueQian.ShortUrl.Models
{
    /// <summary>
    /// 提现
    /// </summary>
    public class Withdrawals : LongIdEntity
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public Verification Verification
        {
            get
            {
                IMongoQuery condition = Query.EQ("UserId", UserId);
                condition = Query.And(condition, Query.EQ("IsDelete", false));

                var model = MongoHelper.Instance.FindOne<Verification>(condition);
                if (model == null)
                    model = new Verification();
                return model;
            }
        }
        public WithdrawalsType State { get; set; }
        public int Amount { get; set; }
        public string Contents { get; set; }
    }
}
