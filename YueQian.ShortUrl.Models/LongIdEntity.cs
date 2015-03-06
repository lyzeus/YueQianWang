using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;

namespace YueQian.ShortUrl.Models
{
    public abstract class LongIdEntity
    {
        [BsonId(IdGenerator = typeof(LongIdGenerator))]
        public long Id { get; set; }

        public bool IsDelete { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreationDate { get; set; }
    }
}
