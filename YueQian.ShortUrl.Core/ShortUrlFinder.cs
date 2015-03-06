using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YueQian.ShortUrl.Models;

namespace YueQian.ShortUrl.Core
{
    public class ShortUrlFinder
    {
        public static WebUrl Find(string shortUrl)
        {
            var condition = MongoDB.Driver.Builders.Query.EQ("ShortUrl", shortUrl);
            var model = MongoHelper.Instance.FindOne<WebUrl>(condition);
            return model;
        }
    }
}
