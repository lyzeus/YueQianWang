using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YueQian.ShortUrl.Models
{
    public class LogBase : LongIdEntity
    {
        public string UserId { get; set; }
        public string Contents { get; set; }
    }
}
