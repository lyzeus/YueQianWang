using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YueQian.ShortUrl.Models;
using YueQian.ShortUrl.Extensions;
using MongoDB.Driver.Builders;
using System.Text;
using MongoDB.Driver;
using YueQian.ShortUrl.Core;

namespace YueQian.ShortUrl.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Create(WebUrl model)
        {
            //if (!Request.IsAuthenticated)
            //    return Json(new { result = false, msg = "请您先<a href=\"/User/Login\">登录</a>" });

            model = new WebUrl() { ShortUrl = YueQian.ShortUrl.Core.ShortUrlGenerator.Generator(6) };
            UpdateModel(model);
            model.BrowerInfo = BrowserInfo.GetFullUserAgent();
            model.Ip = IPAddress.IP;
            if (!model.Url.IsHttpUrl())
                return Json(new { result = false, msg = "您填写网址格式错误,请重新输入!" });
            if (string.IsNullOrEmpty(model.Title))
                model.Title = "跃迁短址";
            model.CreationDate = DateTime.Now;

            var user = System.Web.Security.Membership.GetUser(User.Identity.Name);
            if (Request.IsAuthenticated)
            {
                model.UserId = user.ProviderUserKey.ToString();
                model.UserName = User.Identity.Name;
            }
            else
            {
                model.UserId = "0";
                model.UserName = "公共帐号";
            }
            model.Url = model.Url.EnSureUrl();
            if (Request.IsAuthenticated)
            {
                IMongoQuery condition = Query.Null;
                condition = Query.EQ("UserId", user.ProviderUserKey.ToString());
                condition = Query.And(condition, Query.EQ("Url", model.Url));

                if (MongoHelper.Instance.Has(condition, "WebUrl"))
                    return Json(new { result = false, msg = "该网址您已经添加过了,请重新填写一个吧!" });
            }
            if (MongoHelper.Instance.Save(model))
            {
                StringBuilder msg = new StringBuilder();
                msg.AppendFormat("<a target=\"blank\" href=\"http://yqurl.com/{0}\">yqurl.com/{0}</a>", model.ShortUrl);
                if (!Request.IsAuthenticated)
                    msg.AppendFormat(" <span class=\"reg\">快来<a href=\"/User/register?returnUrl=/User?s={0}\">注册</a>获取丰厚收益和更多功能</span> <a class=\"login\" href=\"/User/Login?returnUrl=/User?s={0}\">登录</a>", model.ShortUrl);

                return Json(new { result = true, msg = msg.ToString() });
            }
            return Json(new { result = false, msg = "提交短址信息错误,请重试!" });
        }


        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult FAQ()
        {
            return View();
        }

        public ActionResult Terms()
        {
            return View();
        }

        public ActionResult Privacy()
        {
            return View();
        }
    }
}
