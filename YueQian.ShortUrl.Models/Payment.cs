using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YueQian.ShortUrl.Models
{
    public class Payment : FinanceEntity
    {
        public long CompanyAccountId { get; set; }
        public CompanyAccount CompanyAccount
        {
            get
            {
                if (CompanyAccountId > 0)
                    return MongoHelper.Instance.FindOne<CompanyAccount>(CompanyAccountId);
                return new CompanyAccount();
            }
        }


        public long WithdrawalsId { get; set; }

        public Withdrawals Withdrawals
        {
            get
            {
                if (CompanyAccountId > 0)
                    return MongoHelper.Instance.FindOne<Withdrawals>(WithdrawalsId);
                return new Withdrawals();
            }
        }

        public int MoneyCount { get; set; }

        /// <summary>
        /// 交易号
        /// </summary>
        public string TradeCode { get; set; }

        /// <summary>
        /// 操作人的Id
        /// </summary>
        public string UserId { get; set; }
        public string UserName { get; set; }
        
    }
}
