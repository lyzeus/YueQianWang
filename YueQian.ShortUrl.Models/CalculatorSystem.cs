using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YueQian.ShortUrl.Models
{
    public class CalculatorSystem : LongIdEntity
    {
        public string Name { get; set; }

        /// <summary>
        /// 签到分数
        /// </summary>
        public int SignIntegral { get; set; }


        /// <summary>
        /// 单次访问积分
        /// </summary>
        public int PerViewIntegral { get; set; }

        /// <summary>
        /// 推广一次分数(即邀请注册分数)
        /// </summary>
        public int SpreadIntegral { get; set; }

        /// <summary>
        /// 管理员加分一次分数
        /// </summary>
        public int AdminAddIntegral { get; set; }
        public string Contents { get; set; }


    }
}
