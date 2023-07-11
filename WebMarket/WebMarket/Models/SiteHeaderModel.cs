using System.Collections.Generic;
using Aware.ECommerce.Model;
using Aware.Regional.Model;

namespace WebMarket.Models
{
    public class SiteHeaderModel
    {
        public List<Category> TopMenuItems { get; set; }
        public bool IsLoggedIn { get; set; }
        public string UserInfo { get; set; }
        public Region CurrentRegion { get; set; }

        public List<int> FavoriteProducts { get; set; }
        public bool AllowProductCompare { get; set; }
        public bool HasNotification { get; set; }
    }
}