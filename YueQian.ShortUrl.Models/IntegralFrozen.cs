using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YueQian.ShortUrl.Models.Enums;

namespace YueQian.ShortUrl.Models
{
    public class IntegralFrozen : LongIdEntity
    {
        public long WithdrawalsId { get; set; }

        public string UserId { get; set; }

        /// <summary>
        /// 积分
        /// </summary>
        public int Integral { get; set; }


        /// <summary>
        /// 备注
        /// </summary>
        public string Contents { get; set; }

        /// <summary>
        /// 解冻原因
        /// </summary>
        public DateTime? ThawDate { get; set; }
        /// <summary>
        /// 解冻原因
        /// </summary>
        public string ThawReason { get; set; }
    }
}
