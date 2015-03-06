using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YueQian.ShortUrl.Models
{
    public class PagerAndSort
    {
        private int _PageIndex = 1;
        private int _PageSize = 20;
        private string _SortField = "_id";
        private SortDirction _SortDirction = SortDirction.Desc;

        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageIndex
        {
            get { return _PageIndex; }
            set { _PageIndex = value; }
        }
        /// <summary>
        /// 每页显示的数量
        /// </summary>
        public int PageSize
        {
            get { return _PageSize; }
            set { _PageSize = value; }
        }
        /// <summary>
        /// 排序字段
        /// </summary>
        public string SortField
        {
            get { return _SortField; }
            set { _SortField = value; }
        }
        /// <summary>
        /// 排序
        /// </summary>
        public SortDirction SortDirction
        {
            get { return _SortDirction; }
            set { _SortDirction = value; }
        }
    }

    public enum SortDirction
    {
        Asc, Desc
    }

    public class JQueryDataTableParamModel
    {
        /// <summary>
        /// Request sequence number sent by DataTable, same value must be returned in response
        /// </summary>       
        public string sEcho { get; set; }

        /// <summary>
        /// Text used for filtering
        /// </summary>
        public string sSearch { get; set; }

        /// <summary>
        /// Number of records that should be shown in table
        /// </summary>
        public int iDisplayLength { get; set; }

        /// <summary>
        /// First record that should be shown(used for paging)
        /// </summary>
        public int iDisplayStart { get; set; }

        /// <summary>
        /// Number of columns in table
        /// </summary>
        public int iColumns { get; set; }

        /// <summary>
        /// Number of columns that are used in sorting
        /// </summary>
        public int iSortingCols { get; set; }

        /// <summary>
        /// Comma separated list of column names
        /// </summary>
        public string sColumns { get; set; }

    }
}
