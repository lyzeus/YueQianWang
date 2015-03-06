using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YueQian.ShortUrl.Models
{
    /// <summary>
    /// 点击量
    /// </summary>
    public class ViewCount : LongIdEntity
    {

        public string UserId { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }

        public string Ip { get; set; }
        /// <summary>
        /// 代理
        /// </summary>
        public string UserAgent { get; set; }
        /// <summary>
        /// 浏览器信息
        /// </summary>
        public string BrowserInfo { get; set; }
        /// <summary>
        /// 登录用户记载用户的Id
        /// </summary>
        public long LoginUserId { get; set; }
    }
}
