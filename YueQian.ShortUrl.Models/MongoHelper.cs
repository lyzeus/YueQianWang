
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace YueQian.ShortUrl.Models
{
    public class MongoHelper : BaseService
    {
        protected override string DataBaseName
        {
            get { return "YueQianWang"; }
        }

        private static readonly MongoHelper _Instance = new MongoHelper();

        public static MongoHelper Instance
        {
            get { return _Instance ?? new MongoHelper(); }
        }

        private MongoHelper() { }

        /// <summary>
        /// 分页和排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">查询条件</param>
        /// <param name="ps">分页和排序</param>
        /// <returns></returns>
        public MongoCursor<T> Find<T>(IMongoQuery query, PagerAndSort ps, out long totalCount)
        {
            var source = Find<T>(query);
            var sourceCount = source.Clone<T>();
            totalCount = sourceCount.Count();
            var sortBy = SortBy.Descending(ps.SortField);
            if (ps.SortDirction == SortDirction.Asc)
                sortBy = SortBy.Ascending(ps.SortField);
            return source.SetSortOrder(sortBy).SetSkip(ps.PageSize * (ps.PageIndex - 1)).SetLimit(ps.PageSize);
        }

        public MongoCursor<BsonDocument> Find(string document, IMongoQuery query, PagerAndSort ps, out long totalCount)
        {
            var source = database.GetCollection(document);
            totalCount = source.Count(query);
            var sortBy = (ps.SortDirction == SortDirction.Asc) ? SortBy.Ascending(ps.SortField) : SortBy.Descending(ps.SortField);
            return source.FindAll().SetSortOrder(sortBy).SetSkip(ps.PageSize * (ps.PageIndex - 1)).SetLimit(ps.PageSize);
        }

        public T FindLast<T>(string collectionName = null)
        {
            var collection = GetCollection<T>(collectionName);
            var model = collection.FindAll().SetSortOrder(SortBy.Descending("CreationDate")).Take<T>(1);
            return (model != null && model.Count() > 0) ? model.First() : Activator.CreateInstance<T>();
        }

        public bool Has(IDictionary<string, string> conditions, string collectionName)
        {
            IMongoQuery query = Query.Null;
            foreach (var item in conditions)
            {
                if (query == Query.Null)
                    query = Query.EQ(item.Key, item.Value);
                else
                    query = Query.And(query, Query.EQ(item.Key, item.Value));
            }
            return Has(query, collectionName);
        }

    }



}
