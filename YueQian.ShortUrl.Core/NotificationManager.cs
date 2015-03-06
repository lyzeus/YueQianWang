using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using YueQian.ShortUrl.Models;
using YueQian.ShortUrl.Models.Enums;

namespace YueQian.ShortUrl.Core
{
    public class NotificationManager
    {
        private List<UserNotification> notifications;

        /// <summary>
        /// 给指定用户发消息
        /// </summary>
        /// <param name="userKey">用户标识</param>
        /// <param name="title">标题</param>
        /// <param name="contents">内容</param>
        /// <param name="isUserId">用户标识是否是UserId</param>
        public NotificationManager(string userKey, string title, string contents, bool isUserId = true)
        {
            var userId = userKey;
            if (!isUserId)
            {
                userId = getUserId(userKey);
            }
            if (!string.IsNullOrEmpty(userKey))
            {
                notifications = new List<UserNotification>();
                notifications.Add(new UserNotification
                {
                    Title = title,
                    MessageType = UserMessageType.系统消息,
                    Contents = contents,
                    CreationDate = DateTime.Now,
                    IsDelete = false,
                    IsRead = false,
                    UserName = userKey,
                    UserId = userId,
                });
            }
        }

        /// <summary>
        /// 给多个用户发通知
        /// </summary>
        /// <param name="userNames">用户名</param>
        /// <param name="messagetype">消息类型</param>
        /// <param name="title">标题</param>
        /// <param name="contents">内容</param>
        public NotificationManager(string[] userNames, UserMessageType messagetype, string title, string contents)
        {
            if (userNames != null && userNames.Count() > 0)
            {
                notifications = new List<UserNotification>();
                var userId = "";
                foreach (var item in userNames)
                {
                    userId = getUserId(item);
                    if (!string.IsNullOrEmpty(userId))
                    {
                        notifications.Add(new UserNotification
                        {
                            Title = title,
                            MessageType = messagetype,
                            Contents = contents,
                            CreationDate = DateTime.Now,
                            IsDelete = false,
                            IsRead = false,
                            UserId = userId,
                            UserName = item
                        });
                    }
                }
            }
        }

        public bool Send()
        {
            if (notifications == null || notifications.Count <= 0) return false;
            foreach (var item in notifications)
            {
                MongoHelper.Instance.Save(item);
            }
            return true;
        }

        private string getUserId(string userName)
        {
            var user = Membership.GetUser(userName);
            if (user != null)
                return user.ProviderUserKey.ToString();
            return "";
        }
    }

    public class NotificationFinder
    {
        public static UserNotification Find(long id)
        {
            var notification = MongoHelper.Instance.FindOne<UserNotification>(id);
            return notification;
        }

        public static bool Read(long id)
        {
            var notification = MongoHelper.Instance.FindOne<UserNotification>(id);
            if (notification != null)
            {
                notification.ReadDate = DateTime.Now;
                notification.IsRead = true;
                return MongoHelper.Instance.Save(notification);
            }
            return false;
        }

        public static long CountUnRead()
        {
            var user = Membership.GetUser(System.Web.HttpContext.Current.User.Identity.Name);
            if (user != null)
            {
                var condition = Query.EQ("IsRead", false);
                condition = Query.And(condition, Query.EQ("UserId", user.ProviderUserKey.ToString()));
                var count = MongoHelper.Instance.Count<UserNotification>(condition);
                return count;
            }
            return 0L;
        }

    }
}
