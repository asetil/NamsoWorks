using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArzTalep.Web.Helper
{
    public class AgeCustomTagHelper : TagHelper
    {
        public int Age { get; set; }
        public string Title { get; set; }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "a";
            output.Attributes.SetAttribute("href", "#");
            output.Content.SetContent(Title + ":" + Age);
            return base.ProcessAsync(context, output);
        }
    }
}
