using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Webdiyer.WebControls.Mvc;

namespace YueQian.ShortUrl.ViewModels
{
    public class ViewModelListBase : ViewModelBase
    {
        public SearchModel SearchModel { get; set; }
        public ListData List { get; set; }

        private static PagerOptions _PagerOptions;
        public virtual PagerOptions PagerOptions
        {
            get
            {
                return _PagerOptions ?? (_PagerOptions = new PagerOptions()
                {
                    CssClass = "pagination",
                    PageIndexParameterName = "page",
                    AutoHide = false,
                    ShowFirstLast = true,
                    FirstPageText = "首页",
                    LastPageText = "末页",
                    PrevPageText = "上一页",
                    NextPageText = "下一页",
                    ShowPrevNext = true,
                    AlwaysShowFirstLastPageNumber = true,
                    NumericPagerItemCount = 6,
                    CurrentPagerItemWrapperFormatString = "<a class=\"current\">{0}</a>"
                });
            }
        }
    }

    public class SearchModel
    {
        public string Keyword { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string CategoryId { get; set; }
    }
}
