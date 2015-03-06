using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YueQian.ShortUrl.Models.Enums;

namespace YueQian.ShortUrl.Models
{
    public class IntegralUsed : LongIdEntity
    {

        public string UserId { get; set; }

        public IntegralUsedType IntegralUsedType { get; set; }

        /// <summary>
        /// 积分
        /// </summary>
        public int Integral { get; set; }


        /// <summary>
        /// 备注
        /// </summary>
        public string Contents { get; set; }
    }
}
