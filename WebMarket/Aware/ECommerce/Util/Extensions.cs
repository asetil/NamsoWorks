using System.Linq;
using Aware.ECommerce.Model;
using Aware.Util;
using System;
using System.Collections.Generic;
using Aware.Dependency;
using Aware.ECommerce.Search;
using Aware.Search;
using Aware.Util.Log;
using Aware.ECommerce.Enums;
using Aware.File.Model;

namespace Aware.ECommerce.Util
{
    public static class Extensions
    {
        public static string AsCommentOwner(this string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var nameParts = name.ToLowerInvariant().Trim().Split(' ');
                if (nameParts.Length >= 2)
                {
                    return string.Format("{0}.. {1}..", nameParts[0].Short(2), nameParts[1].Short(2));
                }
                return string.Format("{0}..", nameParts[0].Short(2));
            }
            return name;
        }

        public static bool HasStore(this Campaign campaign, List<int> storeIDs)
        {
            if (campaign != null && storeIDs.Any())
            {
                if (string.IsNullOrEmpty(campaign.FilterInfo) || campaign.FilterInfo.IndexOf("sid=") == -1) { return true; }
                var filter = campaign.FilterInfo.Split("&").FirstOrDefault(i => i.IndexOf("sid=") > -1);
                if (filter != null)
                {
                    var values = filter.Replace("sid=", "").Split(",");
                    return values.Contains("-1") || storeIDs.Any(s => values.Contains(s.ToString()));
                }
            }
            return false;
        }

        public static Dictionary<string, List<int>> GetFilters(this Campaign campaign)
        {
            if (campaign != null && !string.IsNullOrEmpty(campaign.FilterInfo))
            {
                var filters = campaign.FilterInfo.Split("&");
                if (filters != null && filters.Any())
                {
                    var result = new Dictionary<string, List<int>>();
                    foreach (var filter in filters)
                    {
                        var key = filter.Split("=").FirstOrDefault();
                        var value = filter.Replace(key + "=", "").Split(",").Select(i => i.Int()).ToList();

                        if (!string.IsNullOrEmpty(key) && !result.ContainsKey(key) && value != null && value.Any())
                        {
                            result.Add(key, value);
                        }
                    }
                    return result;
                }
            }
            return null;
        }

        public static bool IsMine(this Campaign campaign, int userID)
        {
            return campaign.OwnerID == 0 || campaign.OwnerID == userID;
        }

        public static Campaign MergeWith(this Campaign campaign, Campaign target)
        {
            if (campaign.ID > 0 && target != null)
            {
                campaign.Scope = target.Scope;
                campaign.DiscountType = target.DiscountType;
            }
            else if (target != null)
            {
                return target.Clone();
            }
            return campaign;
        }

        public static Campaign AsCampaign(this Discount discount)
        {
            var campaign = new Campaign()
            {
                ID = discount.CampaignID,
                Name = "Hediye Kuponu İndirimi",
                Discount = discount.Factor,
                DiscountType = discount.DiscountType == DiscountType.CouponAsRate ? DiscountType.Rate : DiscountType.Amount,
                ExpireDays = 0,
                PublishDate = DateTime.Now
            };
            return campaign;
        }
        
        public static string AsJson(this FileRelation file)
        {
            var result = string.Empty;
            if (file != null && file.ID > 0)
            {
                result = string.Format("[{0}]", Aware.Util.Common.Serialize(file));
            }
            return result;
        }
        
        public static PropertyViewModel AsPropertyView(this VariantProperty variantProperty, List<PropertyValue> childList, bool allowEdit)
        {
            if (variantProperty != null)
            {
                return new PropertyViewModel()
                {
                    Property = new PropertyValue()
                    {
                        ID = variantProperty.ID,
                        Name = variantProperty.Name,
                        Type = PropertyType.VariantProperty
                    },
                    AllowEdit = allowEdit,
                    ChildList = childList
                };
            }
            return new PropertyViewModel();
        }

        public static void Each<T>(this IEnumerable<T> collection, Action<T> action)
        {
            if (collection == null)
                return;

            foreach (T item in collection)
                action(item);
        }
    }
}