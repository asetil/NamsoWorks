namespace Aware.ECommerce.Model.Custom
{
    public class SiteModel
    {
        public bool DisplayComments { get; set; }
        public bool AllowNewComment { get; set; }
        public bool AllowSocialLogin { get; set; }
        public bool AllowSocialShare { get; set; }
        public bool AllowProductCompare { get; set; }
        public bool ShowProductInstallments { get; set; }
        public bool UseMultiLanguage { get; set; }

        public string MailHost { get; set; }
        public string MailPort { get; set; }
        public string MailUser { get; set; }
        public string MailPassword { get; set; }

        public string FacebookApiKey { get; set; }
        public string FacebookApiSecret { get; set; }
        public string GoogleApiKey { get; set; }
        public string GoogleApiSecret { get; set; }
        public string ReCaptchaSecret { get; set; }
    }
}