using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YueQian.ShortUrl.ViewModels;

namespace YueQian.ShortUrl.Admin.ViewModels
{
    public class IndexModel : ViewModelBase
    {

        /// <summary>
        /// 今日访问量
        /// </summary>
        public int TodayViewCount { get; set; }

        public IndexModel()
        {
            PageTitle = "概览";
            SubTitle = "数据 & 统计";

            TodayViewCount = 100;
        }

    }
}
