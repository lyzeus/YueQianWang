using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YueQian.ShortUrl.Core;
using YueQian.ShortUrl.Models;

namespace YueQian.ShortUrl.Web.Controllers
{
    public class ShortUrlController : Controller
    {
        //
        // GET: /ShortUrl/

        public ActionResult Index(string id)
        {
            var query = MongoDB.Driver.Builders.Query.EQ("ShortUrl", id);
            var model = MongoHelper.Instance.FindOne<WebUrl>(query);
            if (model != null)
            {
                var viewcount = new YueQianStatistics(id);
                viewcount.ViewCount();

                return View(model);
            }
            return View("NoItem");
        }
    }
}
