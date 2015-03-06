using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Webdiyer.WebControls.Mvc;

namespace YueQian.ShortUrl.ViewModels
{
    public class ListData : IEnumerable<ListDataItem>, IPagedList
    {
        public IEnumerable<ListDataItem> DataSource { get; private set; }
        public ListData(IEnumerable<ListDataItem> items, int pageIndex = 1, int pageSize = 20, int totalItemCount = 0)
        {
            CurrentPageIndex = pageIndex;
            PageSize = pageSize;
            TotalItemCount = totalItemCount;

            DataSource = items;
        }


        public int CurrentPageIndex { get; set; }

        public int PageSize { get; set; }

        public int TotalItemCount { get; set; }

        IEnumerator<ListDataItem> IEnumerable<ListDataItem>.GetEnumerator()
        {
            return DataSource.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return DataSource.GetEnumerator();
        }
    }
}
