using Aware.Util;

namespace CleanCode.Helper
{
    public class SeoModel
    {
        public SeoModel(string title, string description="")
        {
            Title = string.Format("{0} - Osman Sokuoğlu Yazılım Notları",title).Short(66);
            Description = description;
            Author = "Osman Sokuoğlu";
            Copyright = "Copyright © 2017 Osman Sokuoğlu";
            Locale = "tr_TR";
            SiteName = Config.DomainUrl;
            ImageUrl = string.Format("{0}/res/img/logo.jpg", Config.DomainUrl);
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public string Keywords { get; set; }
        public string Author { get; set; }
        public string Copyright { get; set; }
        public string ContentType { get; set; }
        public string Locale { get; set; }
        public string SiteName { get; set; }
        public string Robots { get; set; }
        public string ImageUrl { get; set; }
        public string PageUrl { get; set; }
        public string CanonicalUrl { get; set; }
    }
}