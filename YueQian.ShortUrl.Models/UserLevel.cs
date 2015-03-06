using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YueQian.ShortUrl.Models
{
    /// <summary>
    /// 用户等级
    /// </summary>
    public class UserLevel : UserEntity
    {
        public MemberShipLevel Level { get; set; }

        /// <summary>
        /// 用户等级变更记录
        /// </summary>
        public List<UserLevelRecord> UserLevelRecord { get; set; }
    }
}
