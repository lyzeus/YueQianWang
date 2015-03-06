using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YueQian.ShortUrl.Models
{
    /// <summary>
    /// 用户等级记录变更记录
    /// </summary>
    public class UserLevelRecord : UserEntity
    {
        public MemberShipLevel Level { get; set; }
        /// <summary>
        /// 变更用户等级时的积分
        /// </summary>
        public int CurrentPoint { get; set; }

        /// <summary>
        /// 本次操作的管理员的Id
        /// </summary>
        public string ManagerId { get; set; }
        public string ManagerName { get; set; }
    }
}
