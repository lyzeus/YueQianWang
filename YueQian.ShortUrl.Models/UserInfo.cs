using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YueQian.ShortUrl.Models
{
    /// <summary>
    /// 用户信息扩展
    /// </summary>
    public class UserInfo : UserEntity
    {
        public string NickName { get; set; }
        public string RealName { get; set; }
        public string Email { get; set; }
        public bool IsComfirm { get; set; }
    }
}
