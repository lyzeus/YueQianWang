using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YueQian.ShortUrl.Models.Enums;

namespace YueQian.ShortUrl.Models
{
    /// <summary>
    /// 用户添加好友记录
    /// </summary>
    public class UserRelationLog : LongIdEntity
    {
        public UserRelationLogType UserRelationLogType { get; set; }

        public string ToUserId { get; set; }

    }
}
