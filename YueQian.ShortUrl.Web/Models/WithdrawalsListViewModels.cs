using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using YueQian.ShortUrl.Models.Enums;
using YueQian.ShortUrl.ViewModels;

namespace YueQian.ShortUrl.Models
{
    public class WithdrawalsListViewModels : ViewModelListBase
    {
        public WithdrawalsListViewModels(int? state, string keyword, DateTime? startDate, DateTime? endDate
                                        , int page, int pageSize)
        {
            var totalItemCount = 0;
            var queries = new List<IMongoQuery>();
            queries.Add(Query.EQ("UserId", CurrentUserId));
            if (startDate.HasValue)
                queries.Add(Query.GTE("CreationDate", startDate.Value));
            if (endDate.HasValue)
                queries.Add(Query.LT("CreationDate", endDate.Value.AddDays(1)));

            var source = MongoHelper.Instance.PagedFind<Withdrawals>(queries.Count > 0 ? Query.And(queries.ToArray()) : Query.Null, out totalItemCount, pageIndex: page, pageSize: pageSize, sortby: SortBy.Descending("CreationDate"));
            string titleFormat = "金额：{0}<br/>{1}";
            List = new ListData(source.Select(s => new ListDataItem
            {
                Id = s.Id,
                Keyword = s.State.ToString(),
                Title = string.Format(titleFormat, s.Amount, s.Verification.ToString()),
                DateTimeString = s.CreationDate.ToString("yyyy年MM月dd日 HH:mm:ss"),
                Description = (s.State == WithdrawalsType.申请被拒) ? string.Format("<a href=\"/User/Withdrawals/{0}\">重新发起</span>", s.Id) : ""
            }), page, pageSize, totalItemCount);

            SearchModel = new SearchModel
            {
                Keyword = keyword,
                CategoryId = (state ?? 0).ToString(),
                StartDate = startDate.HasValue ? startDate.Value.ToString("yyyy-MM-dd") : "",
                EndDate = endDate.HasValue ? endDate.Value.ToString("yyyy-MM-dd") : ""
            };
        }

    }
}
