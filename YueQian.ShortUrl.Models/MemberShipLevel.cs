using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YueQian.ShortUrl.Models
{
    /// <summary>
    /// 用户等级定义
    /// </summary>
    public class MemberShipLevel : LongIdEntity
    {

        //低等级用户级别即时更新
        //高等级(设置某一等级)延迟一个周期(周期长待定)
        public string Name { get; set; }
        public int MinIntegral { get; set; }
        public int MaxIntegral { get; set; }
        public int Integral { get; set; }
        /// <summary>
        /// 系数
        /// </summary>
        public int Coefficient { get; set; }
        public string Contents { get; set; }


        public override bool Equals(object obj)
        {
            if (obj is MemberShipLevel)
                return ((MemberShipLevel)obj).Id == this.Id;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
