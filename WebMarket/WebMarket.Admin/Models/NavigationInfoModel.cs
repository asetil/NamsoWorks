namespace WebMarket.Admin.Models
{
    public class NavigationInfo
    {
        public string PageInfo { get; set; }
        public string ParentInfo { get; set; }
        public string ParentUrl { get; set; }
        public NavigationInfo(string pageInfo, string parentInfo, string parentUrl)
        {
            ParentInfo = parentInfo;
            PageInfo = pageInfo;
            ParentUrl = parentUrl;
        }
    }
}