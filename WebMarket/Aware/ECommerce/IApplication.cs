using Aware.ECommerce.Enums;
using Aware.ECommerce.Model.Custom;
using System.Collections.Generic;
using Aware.Cache;
using Aware.ECommerce.Model;
using Aware.Util;
using Aware.Util.Log;
using Aware.Util.Lookup;

namespace Aware.ECommerce
{
    public interface IApplication
    {
        ICacher Cacher { get; }
        ILogger Log { get; }
        ILookupManager Lookup { get; }
        IWebHelper WebHelper { get; }
        SiteModel Site { get; }
        OrderSettingsModel Order { get; }
        void ClearCache(ItemType cacheType);
        List<string> CheckForBlackList(string title, ref int wordCount);
    }
}
