using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YueQian.ShortUrl.Core
{
    public class BrowserInfo
    {

        public static string GetFullUserAgent()
        {
            return System.Web.HttpContext.Current.Request.UserAgent;
        }

    }
}
