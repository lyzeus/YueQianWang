using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YueQian.ShortUrl.Models;
using YueQian.ShortUrl.ViewModels;

namespace YueQian.ShortUrl.Web.Models
{

    public class SettingModel : ViewModelBase
    {
        public Setting TModel { get; set; }
        public SettingModel()
        {
            TModel = MongoHelper.Instance.FindLast<Setting>();
        }
    }
}