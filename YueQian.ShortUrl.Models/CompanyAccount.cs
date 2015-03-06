using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YueQian.ShortUrl.Models.Enums;

namespace YueQian.ShortUrl.Models
{
    public class CompanyAccount : LongIdEntity
    {
        /// <summary>
        /// 账户类型
        /// </summary>
        public PaymentType PaymentType
        {
            get
            {
                if (PaymentId <= 0)
                    return PaymentType.支付宝;
                return (PaymentType)PaymentId;
            }
        }

        public int PaymentId { get; set; }

        public string AccountBank { get; set; }
        public string Account { get; set; }
        public string RealName { get; set; }
        public string Contents { get; set; }

        public string FullName
        {
            get
            {
                return string.Format("{0}{1}", PaymentType.ToString(), Account);
            }
        }
    }
}
