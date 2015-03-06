using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YueQian.ShortUrl.Models;
using YueQian.ShortUrl.Models.Enums;

namespace YueQian.ShortUrl.Core
{
    public class YueQianStatistics
    {
        public WebUrl Weburl { get; set; }
        public string ShortUrl { get; set; }
        public YueQianStatistics(string shortUrl)
        {
            var model = ShortUrlFinder.Find(shortUrl);
            if (model == null) throw new NotImplementedException("不存在该短址!");

            this.Weburl = model;
            this.ShortUrl = shortUrl;
        }

        public void ViewCount()
        {
            var statistics = new ViewCount
                {
                    BrowserInfo = BrowserInfo.GetFullUserAgent(),
                    CreationDate = DateTime.Now,
                    Ip = IPAddress.IP,
                    Url = ShortUrl,
                    Title = Weburl.Title,
                    UserAgent = BrowserInfo.GetFullUserAgent(),
                    UserId = Weburl.UserId
                };

            //增加一次访问记录
            MongoHelper.Instance.Save(statistics, string.Format("VisitRecord_{0}", DateTime.Now.ToString("yyyy-MM-dd")));

            #region  /*(-_-)*/判断是否要增加有效访问记录以及积分记录/*(-_-)*/

            var condition = Query.EQ("Url", ShortUrl);

            condition = Query.And(condition,
                                  Query.LT("CreationDate", DateTime.Now),
                                  Query.GTE("CreationDate", DateTime.Now.AddHours(-.5)));
            var count = MongoHelper.Instance.Count<ViewCount>(condition);
            if (count <= 0)
            {
                MongoHelper.Instance.Save(statistics);


                #region 增加积分记录

                var integral = new Integral
                {
                    UserId = Weburl.UserId,
                    CalculatorSystem = Application.CalculatorSystem,
                    IntegralType = IntegralType.有效访问,
                    ShortUrl = ShortUrl,
                    CreationDate = DateTime.Now
                };
                MongoHelper.Instance.Save(integral);

                #endregion

                #region 更新用户等级
                var uu = new UserUpdate(Weburl.UserId);
                uu.UpdateUserLevel();
                #endregion

            } 


            #endregion


        }
    }

}
