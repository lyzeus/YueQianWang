using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YueQian.ShortUrl.Models.Enums;

namespace YueQian.ShortUrl.Models
{

    /// <summary>
    /// 关系名称
    /// </summary>
    public class RelationShip : LongIdEntity
    {
        public string Name { get; set; }

    }

    /// <summary>
    /// 关系的属性
    /// </summary>
    public class RelationShipProperty : LongIdEntity
    {
        public long RelationShipId { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public RelationShipDataType DataType { get; set; }
    }

    /// <summary>
    /// 关系属性的值
    /// </summary>
    public class RelationShipPropertyValue : LongIdEntity
    {
        public long RelationShipPropertyId { get; set; }
        public string Value { get; set; }
    }

}
