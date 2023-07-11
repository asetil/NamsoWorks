using System;
using System.Collections.Generic;
using System.Linq;
using Aware;
using Aware.ECommerce.Model;
using Aware.Util;
using Aware.Util.Model;
using Aware.ECommerce.Enums;
using Aware.Util.Enums;
using Aware.Util.Lookup;

namespace WebMarket.Admin.Models
{
    public class CampaignViewModel
    {
        public CampaignViewModel()
        {
            CampaignTemplates = GetTemplates();
        }

        public Campaign Campaign { get; set; }
        public IEnumerable<Campaign> CampaignTemplates { get; set; }
        public List<Item> StoreList { get; set; }
        public List<Item> CategoryList { get; set; }
        public List<Item> PropertyList { get; set; }
        public List<Lookup> StatusList { get; set; }
        public bool AllowEdit { get; set; }
        public Dictionary<string, string> Filters { get; set; }

        public string DiscountSuffix
        {
            get
            {
                if (Campaign == null) { return string.Empty; }
                if (Campaign.DiscountType == DiscountType.Rate) { return "%"; }
                if (Campaign.DiscountType == DiscountType.CouponAsRate) { return "%"; }
                if (Campaign.DiscountType == DiscountType.GiftItem) { return "%"; }
                if (Campaign.DiscountType == DiscountType.Shipping) { return "%"; }
                return Common.Currency;
            }
        }

        private int _selectedTemplate = -1;
        public int SelectedTemplate
        {
            get
            {
                if (Campaign != null && _selectedTemplate == -1)
                {
                    var selectedTemplate = CampaignTemplates.FirstOrDefault(i => i.Scope == Campaign.Scope && i.DiscountType == Campaign.DiscountType);
                    _selectedTemplate = selectedTemplate != null ? selectedTemplate.ID : 0;
                }
                return _selectedTemplate;
            }
            set { _selectedTemplate = value; }
        }

        public bool HasFilter(string filterType, int id)
        {
            if (id > 0 && !string.IsNullOrEmpty(filterType))
            {
                var filterValue = GetFilter(filterType);
                var selectedFilters = filterValue.Split(",");
                return selectedFilters.Contains(id.ToString());
            }
            return false;
        }

        public string GetFilter(string filterType)
        {
            if (Filters != null && Filters.ContainsKey(filterType))
            {
                return Filters[filterType];
            }
            return string.Empty;
        }

        public CampaignViewModel SetFilters(string filterInfo)
        {
            Filters = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(filterInfo))
            {
                var filters = filterInfo.Split("&");
                foreach (var filter in filters)
                {
                    if (!string.IsNullOrEmpty(filter))
                    {
                        var key = filter.Split('=')[0];
                        var value = string.Empty;
                        if (filter.Split('=').Count() > 1) { value = filter.Split('=')[1]; }
                        Filters.Add(key, value);
                    }
                }
            }
            return this;
        }
        private IEnumerable<Campaign> GetTemplates()
        {
            var list = new List<Campaign>();
            list.Add(new Campaign()
            {
                ID = 1,
                Name = "100 TL Üzeri Alışverişlerde Anında  15 TL İndirim Kazanın",
                Description = string.Format("{0} - {1} tarihleri arasında yapacağınız 100 TL ve üzeri tüm alışverişlerde sepetinize anında 15 TL indirim kazanın.", DateTime.Now.ToShortDateString(), DateTime.Now.AddDays(7).ToShortDateString()),
                Scope = CampaignScope.Basket,
                DiscountType = DiscountType.Amount,
                Discount = 15,
                MinimumAmount = 100,
                MinimumQuantity = 0,
                PublishDate = DateTime.Now,
                ExpireDays = 7,
                Status = Statuses.Active,
            });

            list.Add(new Campaign()
            {
                ID = 2,
                Name = "100 TL Üzeri Alışverişlerde Anında  %10 İndirim Kazanın",
                Description = string.Format("{0} - {1} tarihleri arasında yapacağınız 100 TL ve üzeri tüm alışverişlerde sepet tutarınıza anında %10 indirim.", DateTime.Now.ToShortDateString(), DateTime.Now.AddDays(7).ToShortDateString()),
                Scope = CampaignScope.Basket,
                DiscountType = DiscountType.Rate,
                Discount = 10,
                MinimumAmount = 100,
                MinimumQuantity = 0,
                PublishDate = DateTime.Now,
                ExpireDays = 7,
                Status = Statuses.Active,
            });

            list.Add(new Campaign()
            {
                ID = 3,
                Name = "100 TL Üzeri Alışverişlerde Kargo Bedava",
                Description = "100 TL üzeri alışverişlerinizde kargonuz bizden. Hemen alışverişe başlayın kargo bedeli ödemeyin.",
                Scope = CampaignScope.Basket,
                DiscountType = DiscountType.Shipping,
                Discount = 100,
                MinimumAmount = 100,
                MinimumQuantity = 0,
                PublishDate = DateTime.Now,
                ExpireDays = 7,
                Status = Statuses.Active,
            });

            list.Add(new Campaign()
            {
                ID = 4,
                Name = "100 TL Üzeri Alışverişlerde Kargo Sadece 1 TL",
                Description = "100 TL üzeri alışverişlerinizde kargo bedeli sadece 1 TL",
                Scope = CampaignScope.Basket,
                DiscountType = DiscountType.FixedPriceShipping,
                Discount = 1,
                MinimumAmount = 100,
                MinimumQuantity = 0,
                PublishDate = DateTime.Now,
                ExpireDays = 7,
                Status = Statuses.Active,
            });

            list.Add(new Campaign()
            {
                ID = 5,
                Name = "100 TL Üzeri Alışverişlerde 12 TL Tutarında İndirim Kuponu Hediye",
                Description = "Hemen alışverişe başlayın 100 TL üzeri siparişlerinizde sonraki siparişlerinizde kullanabileceğiniz 12TL tutarında indirim kuponu kazanın!",
                Scope = CampaignScope.Basket,
                DiscountType = DiscountType.CouponAsAmount,
                Discount = 12,
                MinimumAmount = 100,
                MinimumQuantity = 1,
                PublishDate = DateTime.Now,
                ExpireDays = 7,
                Status = Statuses.Active
            });

            list.Add(new Campaign()
            {
                ID = 6,
                Name = "250 TL Üzeri Alışverişlerde %20 İndirimli Hediye Kuponu",
                Description = "Hemen alışverişe başlayın 250 TL üzeri siparişlerinizde sonraki siparişlerinizde kullanabileceğiniz %20 indirimli hediye kupon kazanın.",
                Scope = CampaignScope.Basket,
                DiscountType = DiscountType.CouponAsRate,
                Discount = 20,
                MinimumAmount = 250,
                MinimumQuantity = 1,
                PublishDate = DateTime.Now,
                ExpireDays = 7,
                Status = Statuses.Active
            });

            list.Add(new Campaign()
            {
                ID = 7,
                Name = "İçecek Ürünlerinde 3 Al 2 Öde Kampanyası",
                Description = string.Format("{0} - {1} tarihleri arasında içecek reyonunda 3 al 2 öde kampanyası", DateTime.Now.ToShortDateString(), DateTime.Now.AddDays(7).ToShortDateString()),
                Scope = CampaignScope.Basket,
                DiscountType = DiscountType.Rate,
                Discount = 100,
                MinimumAmount = 0,
                MinimumQuantity = 2,
                PublishDate = DateTime.Now,
                ExpireDays = 7,
                FilterInfo = "cid=6",
                Status = Statuses.Active
            });

            return list;
        }
    }
}
