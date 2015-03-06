using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YueQian.ShortUrl.Models
{
    /// <summary>
    /// 用户设备认证
    /// </summary>
    public class UserDevice : UserEntity
    {
        /// <summary>
        /// 已认证设备及认证时间
        /// </summary>
        public Dictionary<Device, DateTime> Devices { get; set; }
    }

    /// <summary>
    /// 设备信息(皆需要加密)
    /// </summary>
    public class Device : LongIdEntity
    {
        /// <summary>
        ///若有号码记录
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 设备信息
        /// </summary>
        public string Contents { get; set; }
    }
}
