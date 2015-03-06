using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YueQian.ShortUrl.Models.Enums;

namespace YueQian.ShortUrl.Models
{
    /// <summary>
    /// 提现失败
    /// </summary>
    public class WithdrawalsReject : LongIdEntity
    {
        public long WithdrawalsId { get; set; }
        public long UserId { get; set; }
        public string Contents { get; set; }
    }
}
