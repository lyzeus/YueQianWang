using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using YueQian.ShortUrl.Models;

namespace YueQian.ShortUrl.ViewModels
{
    public class WebUrlListViewModel : ViewModelListBase
    {
        public WebUrlListViewModel(int? categoryId, string keyword, DateTime? startDate, DateTime? endDate,
            int pageIndex = 1, int pageSize = 20)
        {
            string titleFormat = "<a target=\"_blank\" href=\"/{0}\">{1}({2})</a>";

            var totalItemCount = 0;
            var queries = new List<IMongoQuery>();
            queries.Add(Query.EQ("UserId", CurrentUserId));
            if (categoryId.HasValue)
                queries.Add(Query.EQ("CategoryId", categoryId.Value));
            if (!string.IsNullOrEmpty(keyword))
                queries.Add(Query.Matches("Title", keyword));
            if (startDate.HasValue)
                queries.Add(Query.GTE("CreationDate", startDate.Value));
            if (endDate.HasValue)
                queries.Add(Query.LT("CreationDate", endDate.Value.AddDays(1)));

            var source = MongoHelper.Instance.PagedFind<WebUrl>(queries.Count > 0 ? Query.And(queries.ToArray()) : Query.Null, out totalItemCount, pageIndex: pageIndex, pageSize: pageSize, sortby: SortBy.Descending("CreationDate"));

            List = new ListData(source.Select(s => new ListDataItem
            {
                Id = s.Id,
                Title = string.Format(titleFormat, s.ShortUrl, s.Title, s.FullUrl),
                Keyword = s.FullUrl,
                Description = s.Title,
                Url = s.Url,
                DateTimeString = s.CreationDate.ToString("yyyy/MM/dd")
            }), pageIndex, pageSize, totalItemCount);

            SearchModel = new SearchModel
            {
                Keyword = keyword,
                StartDate = startDate.HasValue ? startDate.Value.ToString("yyyy-MM-dd") : "",
                EndDate = endDate.HasValue ? endDate.Value.ToString("yyyy-MM-dd") : ""
            };
        }
    }
}
