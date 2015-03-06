using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YueQian.ShortUrl.Models.Enums;

namespace YueQian.ShortUrl.Models
{
    /// <summary>
    /// 用户消息通知
    /// </summary>
    public class UserNotification : UserEntity
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public UserMessageType MessageType { get; set; }
        public string UserName { get; set; }
        public string Title { get; set; }
        public string Contents { get; set; }

        public bool IsRead { get; set; }

        /// <summary>
        /// 阅读消息的时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? ReadDate { get; set; }
    }
}
