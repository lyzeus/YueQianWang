using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YueQian.ShortUrl.Models
{
    /// <summary>
    /// 用户发布签名说说
    /// </summary>
    public class UserTalk : UserEntity
    {
        public string Contents { get; set; }
    }
}
