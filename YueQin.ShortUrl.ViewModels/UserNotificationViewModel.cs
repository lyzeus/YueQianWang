using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using YueQian.ShortUrl.Models;

namespace YueQian.ShortUrl.ViewModels
{
    public class UserNotificationViewModel : ViewModelListBase
    {
        private const string unread = "<div id=\"rn{0}\" onclick=\"readNotification({0})\" class=\"unread\">{1}<br/>{2}<div>";
        private const string read = "<div id=\"rn{0}\"class=\"read\">{1}<br/>{2}<div>";

        public UserNotificationViewModel(DateTime? startDate, DateTime? endDate,
            int pageIndex = 1, int pageSize = 20, bool isAdminView = false)
        {
            if (isAdminView)
            {
                PageTitle = "消息通知";
                SubTitle = "消息 & 管理";
            }

            var totalItemCount = 0;
            var queries = new List<IMongoQuery>();
            if (!isAdminView) queries.Add(Query.EQ("UserId", CurrentUserId));
            if (startDate.HasValue)
                queries.Add(Query.GTE("CreationDate", startDate.Value));
            if (endDate.HasValue)
                queries.Add(Query.LT("CreationDate", endDate.Value.AddDays(1)));

            var source = MongoHelper.Instance.PagedFind<UserNotification>(queries.Count > 0 ? Query.And(queries.ToArray()) : Query.Null, out totalItemCount, pageIndex: pageIndex, pageSize: pageSize, sortby: SortBy.Descending("CreationDate"));


            List = new ListData(source.Select(s => new ListDataItem
            {
                Id = s.Id,
                Keyword = s.MessageType.ToString(),
                Title = string.Format(s.IsRead ? read : unread, s.Id, s.Title, s.Contents),
                DateTimeString = s.CreationDate.ToString(),
                Description = s.IsRead ? string.Format("已读({0})", s.ReadDate.Value.ToString()) : "未读"
            }), pageIndex, pageSize, totalItemCount);

            SearchModel = new SearchModel
            {
                StartDate = startDate.HasValue ? startDate.Value.ToString("yyyy-MM-dd") : "",
                EndDate = endDate.HasValue ? endDate.Value.ToString("yyyy-MM-dd") : ""
            };
        }
    }
}
