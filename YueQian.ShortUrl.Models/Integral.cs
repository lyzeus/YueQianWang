using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YueQian.ShortUrl.Models.Enums;

namespace YueQian.ShortUrl.Models
{
    /// <summary>
    /// 用户积分
    /// </summary>
    public class Integral : UserEntity
    {
    
        public IntegralType IntegralType { get; set; }

        /// <summary>
        /// 短址积分时填写
        /// </summary>
        public string ShortUrl { get; set; }

        //计算方式
        public long CalculatorSystem { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Contents { get; set; }

        public int Result
        {
            get
            {
                var cs = MongoHelper.Instance.FindOne<CalculatorSystem>(CalculatorSystem);
                if (cs == null) return 0;
                switch (IntegralType)
                {
                    case IntegralType.签到:
                        return cs.SignIntegral;
                    case IntegralType.有效访问:
                        return cs.PerViewIntegral;
                    case IntegralType.用户推广:
                        return cs.SpreadIntegral;
                    default:
                        return 0;
                }

            }
        }
    }
}
