using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YueQian.ShortUrl.Extensions;
using YueQian.ShortUrl.Models;

namespace YueQian.ShortUrl.Core
{

    public class ShortUrlGenerator
    {
        private const string characters = "123456789abcdefghijklmnopqrstuvwxyz";

        public static string Generator(int length = 6)
        {
            var result = "";
            result = characters.Random(length);

            while (Exist(result))
            {
                result = characters.Random(length);
            }
            return result;
        }


        public static bool Exist(string url)
        {
            var condition = new Dictionary<string, string>();
            condition.Add("ShortUrl", url);
            return MongoHelper.Instance.Has(condition, "WebUrl");
        }

    }
}
