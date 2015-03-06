using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YueQian.ShortUrl.Models
{
    public class ArticleBase : LongIdEntity
    {
        public string Title { get; set; }
        public string Keywords { get; set; }
        public string Description { get; set; }

    }
}
