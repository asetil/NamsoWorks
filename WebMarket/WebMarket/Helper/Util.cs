using System;
using System.Text;
using System.Linq;
using Aware.ECommerce.Model;
using System.Collections.Generic;
using Aware.Util;
using Aware.Util.Model;
using Aware.Util.Slider;


namespace WebMarket.Helper
{
    public static class Util
    {
        public static string DrawProductHierarchicalCategories(IEnumerable<Category> categories)
        {
            var content = new StringBuilder();
            var ind = 1;
            foreach (var item in categories)
            {
                content.AppendFormat("<a href='/{0}-urunleri-{1}'>{2}</a> {3} ", item.Name.ToSeoUrl(), item.ID, item.Name, ind == categories.Count() ? string.Empty : "<i class='fa fa-angle-right'></i>");
                ind++;
            }
            return content.ToString().TrimEnd();
        }

        public static Slider GetMainSlider(List<SliderItem> items)
        {
            var result = new Slider(string.Empty, 3000, "main-carousel");
            if (items != null && items.Any())
            {
                foreach (var item in items)
                {
                    var imageDomain = item.ImagePath.StartsWith("http://") || item.ImagePath.StartsWith("https://") || item.ImagePath.StartsWith("//") ? string.Empty : Config.ImageRepository;
                    var html = string.Format("<img src='{0}{1}' alt='{2}' title='{2}'/>", imageDomain, item.ImagePath, item.Title);
                    if (!string.IsNullOrEmpty(item.Url))
                    {
                        html = string.Format("<a href='{0}' class='carousel-link'>{1}</a>", item.Url, html);
                    }
                    result.AddItem(item.ID, html);
                }
            }
            else
            {
                result.AddItem(0, "/Slider/0.png");
            }
            return result;
        }
    }
}
