using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YueQian.ShortUrl.Models
{
    public class WebUrl : ArticleBase
    {
        public long CategoryId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        /// <summary>
        /// 原址
        /// </summary>
        public string Url { get; set; }
        public string FullUrl { get { return string.Format("http://yqurl.com/{0}", ShortUrl); } }
        public string ShortUrl { get; set; }
        public string Ip { get; set; }
        public string BrowerInfo { get; set; }
        /// <summary>
        /// 有效访问量
        /// </summary>
        public long ViewCount
        {
            get
            {
                if (string.IsNullOrEmpty(ShortUrl)) return 0;
                var condition = MongoDB.Driver.Builders.Query.EQ("Url", ShortUrl);
                return MongoHelper.Instance.Count<ViewCount>(condition);
            }
        }
    }
}
