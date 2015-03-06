using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YueQian.ShortUrl.ViewModels;

namespace YueQian.ShortUrl.Admin.ViewModels
{
    public class AdminCommonModel : ViewModelBase
    {

        public string UserName { get; set; }
        public string SearchKeyWord { get; set; }

        public AdminCommonModel()
        {
            PageTitle = "概览";
            SubTitle = "数据 & 统计";
        }

    }
}
