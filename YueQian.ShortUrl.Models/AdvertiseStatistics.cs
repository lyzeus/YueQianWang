using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YueQian.ShortUrl.Models
{
    public class AdvertiseStatistics : LongIdEntity
    {
        public string AdvertiseNumber { get; set; }
        public string ClientIpAddress { get; set; }
    }
}
