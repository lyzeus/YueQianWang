using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YueQian.ShortUrl.Models.Enums;

namespace YueQian.ShortUrl.Models
{
    /// <summary>
    /// 提现
    /// </summary>
    public class WithdrawalsLog : LogBase
    {
        public long WithdrawalsId { get; set; }
    }
}
