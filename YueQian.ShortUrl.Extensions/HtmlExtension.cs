using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Webdiyer.WebControls.Mvc;
using YueQian.ShortUrl.Models;
using YueQian.ShortUrl.Models.Enums;
using YueQian.ShortUrl.ViewModels;

namespace YueQian.ShortUrl.Extensions
{
    public static class HtmlExtension
    {
        public static MvcHtmlString Pager(this HtmlHelper helper)
        {
            var model = (ViewModelListBase)helper.ViewData.Model;
            return helper.Pager(model.List, model.PagerOptions);
        }

        public static MvcHtmlString CalculatorSystemSelect(this HtmlHelper htmlHelper, string name, long selected, object htmlAttributes)
        {
            var options = MongoHelper.Instance.Find<CalculatorSystem>(MongoDB.Driver.Builders.Query.Null);
            if (options != null && options.Count() > 0)
            {
                var items = options.OrderByDescending(s => s.CreationDate).Select(s => new SelectListItem
                {
                    Selected = (s.Id == selected),
                    Text = s.Name,
                    Value = string.Format("{0}&一次有效访问积{1}分,签到一次积{2}分", s.Id.ToString(), s.PerViewIntegral, s.SignIntegral)
                });

                return htmlHelper.DropDownList(name, items, htmlAttributes);
            }
            return htmlHelper.DropDownList(name, new List<SelectListItem> { new SelectListItem { Text = "暂无数据", Value = "&" } }, htmlAttributes);
        }

        public static MvcHtmlString PaymentSelect(this HtmlHelper htmlHelper, string name, int selected, object htmlAttributes, string label = "")
        {
            return EnumSelect<PaymentType>(htmlHelper, name, selected, htmlAttributes, label: label);
        }

        public static MvcHtmlString EnumSelect<T>(this HtmlHelper htmlHelper, string name, int selected, object htmlAttributes, string label = "")
        {
            var type = typeof(T);

            var values = Enum.GetValues(type);
            var options = new List<SelectListItem>();

            foreach (int item in values)
            {
                options.Add(new SelectListItem
                {
                    Text = Enum.GetName(type, item),
                    Value = item.ToString(),
                    Selected = (item == selected),
                });
            }
            if (!string.IsNullOrEmpty(label))
                return htmlHelper.DropDownList(name, options, label, htmlAttributes);
            return htmlHelper.DropDownList(name, options, htmlAttributes);
        }

        public static MvcHtmlString CompayAccountSelect(this HtmlHelper htmlHelper, string name, int selected, object htmlAttributes, string label = "")
        {
            var source = MongoHelper.Instance.Find<CompanyAccount>(MongoDB.Driver.Builders.Query.Null);

            var options = new List<SelectListItem>();

            foreach (var item in source)
            {
                options.Add(new SelectListItem
                {
                    Text = item.FullName,
                    Value = item.Id.ToString(),
                    Selected = (item.Id == selected),
                });
            }
            if (!string.IsNullOrEmpty(label))
                return htmlHelper.DropDownList(name, options, label, htmlAttributes);
            return htmlHelper.DropDownList(name, options, htmlAttributes);
        }

        public static MvcHtmlString AdvtisementPositionSelect(this HtmlHelper htmlHelper, string name, int selected, object htmlAttributes, bool includePrice = false, string label = "")
        {
            var source = MongoHelper.Instance.Find<AdvertisePosition>(MongoDB.Driver.Builders.Query.Null);

            var options = new List<SelectListItem>();

            foreach (var item in source)
            {
                options.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = includePrice ? string.Format("{0}&{1}&{2}&{3}", item.PositionNumber, item.Price, item.Width, item.Height) : item.PositionNumber,
                    Selected = (item.Id == selected),
                });
            }
            if (!string.IsNullOrEmpty(label))
                return htmlHelper.DropDownList(name, options, label, htmlAttributes);
            return htmlHelper.DropDownList(name, options, htmlAttributes);
        }



    }
}
