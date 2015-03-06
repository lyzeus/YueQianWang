using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YueQian.ShortUrl.Models
{

    public class LongIdGenerator : IIdGenerator
    {
        private const string idCollectionName = "CollectionIds";

        public object GenerateId(object container, object document)
        {
            var collection = (MongoCollection)container;

            var idSequenceCollection = collection.Database.GetCollection(idCollectionName);

            var query = Query.EQ("CollectionName", collection.Name);
            var update = Update.Inc("Index", 1L);

            return Convert.ToInt64(idSequenceCollection.FindAndModify(query, null, update, true, true).ModifiedDocument["Index"]);
        }

        public bool IsEmpty(object id)
        {
            if (id == null) return true;
            return Convert.ToInt64(id) <= 0L;
        }
    }
}
