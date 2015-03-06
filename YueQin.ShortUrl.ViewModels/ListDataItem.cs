using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YueQian.ShortUrl.ViewModels
{
    public class ListDataItem
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Keyword { get; set; }
        public string Description { get; set; }
        public string DateTimeString { get; set; }

    }
}
