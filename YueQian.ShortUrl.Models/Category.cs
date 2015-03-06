using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YueQian.ShortUrl.Models.Enums;

namespace YueQian.ShortUrl.Models
{
    public class Category : LongIdEntity
    {
        public string Name { get; set; }
        public long ParentId { get; set; }
        public CategoryType Type { get; set; }
        public string Description { get; set; }
    }
}
