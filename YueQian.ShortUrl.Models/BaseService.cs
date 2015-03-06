using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YueQian.ShortUrl.Models
{
    public abstract class BaseService
    {
        protected abstract string DataBaseName { get; }

        /// <summary>
        /// 数据连接-数据库
        /// </summary>
        protected MongoDatabase database { get { return server.GetDatabase(DataBaseName); } }

        private static MongoClient client;
        private static MongoServer server;

        static BaseService()
        {
            client = new MongoClient();
            server = client.GetServer();
        }


        public MongoCollection<T> GetCollection<T>(string collectionName = null)
        {
            return database.GetCollection<T>(collectionName ?? typeof(T).Name);
        }

        public bool Save<T>(T entity, string collectionName = null) where T : LongIdEntity
        {
            var collection = GetCollection<T>(collectionName);
            return collection.Save(entity).Ok;
        }

        public bool Delete<T>(string collectionName = null, params long[] id) where T : LongIdEntity
        {
            return Delete<T>(Query.In("_id", new BsonArray(id)), collectionName);
        }

        public bool Delete<T>(IMongoQuery query, string collectionName = null) where T : LongIdEntity
        {
            var collection = GetCollection<T>(collectionName);
            return collection.Remove(query).Ok;
        }

        /// <summary>
        /// 标记删除一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">Id</param>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public bool SafeDelete<T>(long id, string collectionName = null) where T : LongIdEntity
        {
            var query = Query.EQ("_id", id);
            var update = Update.Set("IsDelete", true);
            var collection = GetCollection<T>(collectionName);
            return collection.Update(query, update).Ok;
        }

        public bool Delete(string collectionName, params long[] ids)
        {
            var collction = database.GetCollection(collectionName);
            return collction.Remove(Query.In("_id", new BsonArray(ids))).Ok;
        }

        public T FindOne<T>(IMongoQuery query, string collectionName = null) where T : LongIdEntity
        {
            var collection = GetCollection<T>(collectionName);
            return collection.FindOneAs<T>(query);
        }

        public T FindOne<T>(long id, string collectionName = null) where T : LongIdEntity
        {
            return FindOne<T>(Query.EQ("_id", id), collectionName);
        }


        public MongoCursor<T> Find<T>(IMongoQuery query, string collectionName = null)
        {
            var collection = GetCollection<T>(collectionName);
            return collection.Find(query);
        }

        public MongoCursor<T> PagedFind<T>(IMongoQuery query, out int totalCount, string collectionName = null, int pageIndex = 1, int pageSize = 20, IMongoSortBy sortby = null) where T : LongIdEntity
        {
            var condition = SimpleQueryBuider(query);
            var source = MongoHelper.Instance.Find<T>(condition, collectionName);
            var items = source.Clone<T>();
            totalCount = (int)items.Count();
            return source.SetSortOrder(sortby)
                         .SetSkip(pageSize * (pageIndex - 1))
                         .SetLimit(pageSize);
        }

        public long Count<T>(IMongoQuery query, string collectionName = null)
        {
            var collection = GetCollection<T>(collectionName);
            return collection.Count(query);
        }

        public bool Has<T>(IMongoQuery query, string collectionName = null)
        {
            var collection = GetCollection<T>(collectionName);
            return collection.Count(SimpleQueryBuider(query)) > 0;
        }

        public bool Has(IMongoQuery query, string collectionName)
        {

            var collection = database.GetCollection(collectionName);
            return collection.Count(SimpleQueryBuider(query)) > 0;
        }

        private IMongoQuery SimpleQueryBuider(IMongoQuery query)
        {
            var condition = Query.EQ("IsDelete", false);            
            if (query != Query.Null)
                condition = Query.And(condition, query);

            return condition;
        }

    }
}
