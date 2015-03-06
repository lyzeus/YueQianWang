using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YueQian.ShortUrl.Models;

namespace YueQian.ShortUrl.Core
{
    public class LogManager
    {
        private const string LOG_COLLECTION_NAME = "Logs";

        public static bool AddLog<T>(T log) where T : LogBase
        {
            return MongoHelper.Instance.Save(log, LOG_COLLECTION_NAME);
        }

        public static IEnumerable<T> GetLogs<T>(string key, MongoDB.Bson.BsonValue value)
        {
            IMongoQuery condition = Query.EQ(key, value);

            return MongoHelper.Instance.Find<T>(condition, LOG_COLLECTION_NAME);
        }
    }
}
