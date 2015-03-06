using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace YueQian.ShortUrl.Models
{
    public class Advertisement : LongIdEntity
    {
        public string AdvertiseNumber { get; set; }
        public string PositionNumber { get; set; }
        /// <summary>
        /// 链接地址
        /// </summary>
        public string Url { get; set; }
        public AdvertisePosition Position
        {
            get
            {
                IMongoQuery query = Query.EQ("PositionNumber", PositionNumber);
                var position = MongoHelper.Instance.FindOne<AdvertisePosition>(query);
                if (position != null)
                    return position;
                return new AdvertisePosition { Name = "不存在该位置" };
            }
        }
        public decimal ActualPrice { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? StartTime { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? EndTime { get; set; }
        public YueQian.ShortUrl.Models.Enums.AdvType AdvType { get; set; }
        public string FilePath { get; set; }
        public string Contents { get; set; }
        public bool IsAvailable { get; set; }
    }
}
