using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YueQian.ShortUrl.Extensions
{
    public static class StringExtension
    {
        public static bool IsEmail(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;
            var regex = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            return regex.IsMatch(str);
        }


        public static bool IsHttpUrl(this string str, bool startWithHTTP = false)
        {
            if (string.IsNullOrEmpty(str))
                return false;
            var regex = new Regex(@"((http|https):\/\/)?[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?", RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace | RegexOptions.ExplicitCapture);
            if (startWithHTTP)
                regex = new Regex(@"(http|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?", RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace | RegexOptions.ExplicitCapture);
            return regex.IsMatch(str);
        }

        public static string EnSureUrl(this string str)
        {
            if (!str.IsHttpUrl(true))
                return string.Format("http://{0}", str);
            return str;
        }


        /// <summary>
        /// 在指定字符串随机取一个字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static char RandomOne(this string str)
        {
            if (string.IsNullOrEmpty(str))
                throw new NotImplementedException();
            Random random = new Random((int)DateTime.Now.Ticks);

            return str[random.Next(0, str.Length - 1)];
        }

        /// <summary>
        /// 指定字符串中取指定长度字符
        /// </summary>
        /// <param name="str"></param>
        /// <param name="length">长度</param>
        /// <returns></returns>
        public static string Random(this string str, int length)
        {
            if (string.IsNullOrEmpty(str))
                throw new NotImplementedException();
            if (str.Length < length)
                throw new NotImplementedException();
            var result = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                result.Append(str.RandomOne());
                System.Threading.Thread.Sleep(100);
            }
            return result.ToString();
        }
    }
}
