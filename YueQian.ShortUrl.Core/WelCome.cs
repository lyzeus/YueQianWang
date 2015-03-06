using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YueQian.ShortUrl.Core
{
    public class WelCome
    {
        /// <summary>
        /// 获取此时此刻对应的欢迎词
        /// </summary>
        /// <returns></returns>
        public static string GetSalutatroy()
        {
            var result = "";

            var currentHour = DateTime.Now.Hour;

            if (currentHour >= 6 & currentHour < 11)
            {
                result = "早上好，新的一天，新的开始";
            }
            else if (currentHour >= 11 & currentHour < 14)
            {
                result = "中午了，来顿可口的午餐吧";
            }
            else if (currentHour >= 14 & currentHour < 19)
            {
                result = "Tea Time!泡杯咖啡放松一下";
            }
            else if (currentHour >= 19 & currentHour < 23)
            {
                result = "忙碌了一天，可以在晚上放松一下，顺便查看今天跃迁网给您带来了多少收益哟";
            }
            else
            {
                result = "夜深了，还不休息，也是蛮拼的";
            }

            return result;
        }
    }
}
