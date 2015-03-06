using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YueQian.ShortUrl.Models;

namespace YueQian.ShortUrl.Core
{
    public class Application
    {
        private static Setting _CurrentSetting;

        public static int CalculatorSystem
        {
            get
            {
                return _CurrentSetting.CalculatorSystem;
            }
        }

        static Application()
        {
            var query = MongoDB.Driver.Builders.Query.EQ("IsDelete", false);
            var setting = MongoHelper.Instance.FindOne<Setting>(query);
            if (setting == null)
            {
                setting = new Setting
                {
                    CalculatorSystem = 1
                };
            }
            _CurrentSetting = setting;
        }
    }
}
