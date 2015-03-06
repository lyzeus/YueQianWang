using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YueQian.ShortUrl.Models
{
    public class AdvertisePosition : LongIdEntity
    {
        public string PositionNumber { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public decimal Price { get; set; }
    }
}
