using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YueQian.ShortUrl.Models.Enums;

namespace YueQian.ShortUrl.Models
{
    /// <summary>
    /// 用户认证信息
    /// </summary>
    public class Verification : UserEntity
    {
       /// <summary>
       /// 支付方式
       /// </summary>
        public PaymentType Payment
        {
            get
            {
                if (PaymentId <= 0)
                    return PaymentType.支付宝;
                return (PaymentType)PaymentId;
            }
        }
        /// <summary>
        /// 支付方式Id
        /// </summary>
        public int PaymentId { get; set; }
        /// <summary>
        /// 开卡行(当支付方式为银行时填写)
        /// </summary>
        public string AccountBank { get; set; }
        /// <summary>
        /// 帐号
        /// </summary>
        public string Account { get; set; }

        public string RealName { get; set; }
        /// <summary>
        /// 身份证号码
        /// </summary>
        public string IdentityNumber { get; set; }
        /// <summary>
        /// 手机(一般用于接收短信)
        /// </summary>
        public string MobilePhone { get; set; }
        public string Contents { get; set; }

        /// <summary>
        /// 输出基本信息
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            result.AppendFormat("付款方式:{0}<br/>", Payment.ToString());
            if (PaymentId > 1) { result.AppendFormat("开户行:{0}<br/>", AccountBank); }
            result.AppendFormat("账户:{0}<br/>", Account);
            result.AppendFormat("账户名:{0}<br/>", RealName);
            return result.ToString();
        }
    }
}
