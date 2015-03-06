using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YueQian.ShortUrl.Models;

namespace YueQian.ShortUrl.Web.Helpers
{
    public static class AdvertisementHelper
    {
        /// <summary>
        /// 输出一个链接，0占位链接地址 1占位Title 2占位内容
        /// </summary>
        private const string LINK = "<a target=\"_blank\" href=\"{0}\" title=\"{1}\">{2}</a>";
        /// <summary>
        /// 输出flash的广告 0占位flash地址 1占位宽度  2占位高度
        /// </summary>  
        private const string AD_FLASH = "<embed height=\"200\" flashvars=\"\" pluginspage=\"http://www.adobe.com/go/getflashplayer\" src=\"{0}\" type=\"application/x-shockwave-flash\" width=\"{1}\" height=\"{2}\"></embed>";
        /// <summary>
        /// 输出图片的广告 0占位图片地址 1占位宽度  2占位高度
        /// </summary>
        private const string AD_IMAGE = "<img src=\"{0}\" alt=\"{1}\" width=\"{1}\" height=\"{2}\"/>";

        private static Advertisement GetAdvertisement(string code)
        {
            IMongoQuery condition = Query.EQ("PositionNumber", code);
            condition = Query.And(condition, Query.EQ("IsAvailable", true));
            var advs = MongoHelper.Instance.Find<Advertisement>(condition);
            if (advs != null)
            {
                Func<Advertisement, bool> timeCondition = s => (s.StartTime.HasValue ? s.StartTime.Value < DateTime.Now : true)
                                                            && (s.EndTime.HasValue ? s.EndTime.Value >= DateTime.Now : true);

                var adv = advs.FirstOrDefault(timeCondition);
                if (adv != null)
                    return adv;
            }

            return new Advertisement { Contents = "不存在该位置的广告" };
        }



        /// <summary>
        /// 生成广告html代码
        /// </summary>
        /// <param name="adv"></param>
        /// <returns></returns>
        private static string OrganizeAd(Advertisement adv)
        {
            if (adv.Id <= 0) return "";
            var content = "";
            switch (adv.AdvType)
            {
                default:
                case YueQian.ShortUrl.Models.Enums.AdvType.text:
                    content = adv.Contents;
                    if (!string.IsNullOrEmpty(adv.Url))
                        content = string.Format(LINK, adv.Url, adv.AdvertiseNumber, content);
                    return content;
                case YueQian.ShortUrl.Models.Enums.AdvType.imge:
                    content = string.Format(AD_IMAGE, adv.FilePath, adv.Position.Width, adv.Position.Height);
                    if (!string.IsNullOrEmpty(adv.Url))
                        content = string.Format(LINK, adv.Url, adv.AdvertiseNumber, content);
                    return content;
                case YueQian.ShortUrl.Models.Enums.AdvType.flash:
                    content = string.Format(AD_FLASH, adv.FilePath, adv.Position.Width, adv.Position.Height);
                    if (!string.IsNullOrEmpty(adv.Url))
                        content = string.Format(LINK, adv.Url, adv.AdvertiseNumber, content);
                    return content;
            }

        }

        /// <summary>
        /// 左侧对联
        /// </summary>
        public static IHtmlString LeftCouplet(this HtmlHelper helper)
        {
            return helper.RenderHtmlAd("LeftCouplet");
        }

        /// <summary>
        /// 左侧对联
        /// </summary>
        public static IHtmlString RightCouplet(this HtmlHelper helper)
        {
            return helper.RenderHtmlAd("RightCouplet");
        }

        /// <summary>
        /// 通栏
        /// </summary>
        public static IHtmlString Banner(this HtmlHelper helper)
        {
            return helper.RenderHtmlAd("Banner");
        }

        /// <summary>
        /// 通栏
        /// </summary>
        public static IHtmlString Message(this HtmlHelper helper)
        {
            return helper.RenderHtmlAd("Message");
        }

        /// <summary>
        /// 输出广告HTML
        /// </summary>
        /// <param name="code">广告编码</param>
        public static IHtmlString RenderHtmlAd(this HtmlHelper helper, string code)
        {
            return helper.Raw(OrganizeAd(GetAdvertisement(code)));
        }
    }
}