using System;
using System.Collections.Generic;
using System.Linq;
using Aware.ECommerce.Model;
using Aware.Util;
using Ploeh.AutoFixture;
using Aware.Util.Enums;
using Aware.ECommerce.Enums;

namespace Aware.Test.ECommerce
{
    public class CampaignTestHelper
    {
        private IFixture _fixture;

        public CampaignTestHelper(IFixture fixture)
        {
            _fixture = fixture;
        }

        public Basket GetBasket(int itemCount, decimal maxPrice, decimal minPrice = 0, int storeID = 0)
        {
            var r = new Random();
            var basket = new Basket
            {
                Status = Statuses.Active,
                ID = _fixture.Create<int>(),
                Items = _fixture.CreateMany<BasketItem>(itemCount)
                    .Select(i =>
                    {
                        i.StoreID = storeID > 0 ? storeID : i.StoreID;
                        i.Status = Statuses.Active;
                        i.Price = r.Next((int) minPrice, (int) maxPrice);
                        i.Quantity = 1;
                        i.GrossTotal = i.Price*i.Quantity;
                        return i;
                    }).ToList()
            };

            return basket;
        }

        public Basket GetBasketWithName(string basketName, int itemCount, decimal maxPrice, decimal minPrice = 0, int storeID = 0)
        {
            if (basketName == "basket_filter_test")
            {
                var basket = GetBasket(itemCount, maxPrice, minPrice, storeID);
                basket.Items = basket.Items.Select(i =>
                {
                    i.Product.CategoryID = i.ID % 10;
                    i.Product.PropertyInfo = "[{'ID':'" + i.ID % 10 + "','Name':'Marka','Value':'Ülker','SortOrder':'1','Type':3}]";
                    return i;
                }).ToList();

                return basket;
            }

            if (basketName == "basket_filter_fail_test")
            {
                var basket = GetBasket(itemCount, maxPrice, minPrice, storeID);
                basket.Items = basket.Items.Select(i =>
                {
                    i.Product.CategoryID = 122;
                    i.Product.PropertyInfo = "[]";
                    return i;
                }).ToList();

                return basket;
            }
            return null;
        }


        public Campaign GetCampaign(string campaignType, decimal discount = 12, decimal minAmount = 80, decimal minQuantity = 0)
        {

            if (campaignType == "all_amount")
            {
                return new Campaign()
                {
                    ID = 1,
                    Name = "100 TL Üzeri Alýþveriþlerde Anýnda  15 TL Ýndirim Kazanýn",
                    Description = string.Format("{0} - {1} tarihleri arasýnda yapacaðýnýz 100 TL ve üzeri tüm alýþveriþlerde sepetinize anýnda 15 TL indirim kazanýn.",
                        DateTime.Now.ToShortDateString(), DateTime.Now.AddDays(7).ToShortDateString()),
                    Scope = CampaignScope.Basket,
                    DiscountType = DiscountType.Amount,
                    Discount = discount,
                    MinimumAmount = minAmount,
                    MinimumQuantity = minQuantity,
                    PublishDate = DateTime.Now,
                    ExpireDays = 7,
                    Status = Statuses.Active,
                };
            }
            else if (campaignType == "all_rate")
            {
                return new Campaign()
                {
                    ID = 2,
                    Name = "100 TL Üzeri Alýþveriþlerde Anýnda  %10 Ýndirim Kazanýn",
                    Description = string.Format("{0} - {1} tarihleri arasýnda yapacaðýnýz 100 TL ve üzeri tüm alýþveriþlerde sepet tutarýnýza anýnda %10 indirim.", DateTime.Now.ToShortDateString(), DateTime.Now.AddDays(7).ToShortDateString()),
                    Scope = CampaignScope.Basket,
                    DiscountType = DiscountType.Rate,
                    Discount = discount,
                    MinimumAmount = minAmount,
                    MinimumQuantity = minQuantity,
                    PublishDate = DateTime.Now,
                    ExpireDays = 7,
                    Status = Statuses.Active,
                };
            }
            else if (campaignType == "all_shipping")
            {
                return new Campaign()
                {
                    ID = 3,
                    Name = "100 TL Üzeri Alýþveriþlerde Kargo Bedava",
                    Description = "100 TL üzeri alýþveriþlerinizde kargonuz bizden. Hemen alýþveriþe baþlayýn kargo bedeli ödemeyin.",
                    Scope = CampaignScope.Basket,
                    DiscountType = DiscountType.Shipping,
                    Discount = discount,
                    MinimumAmount = minAmount,
                    MinimumQuantity = minQuantity,
                    PublishDate = DateTime.Now,
                    ExpireDays = 7,
                    Status = Statuses.Active,
                };
            }
            else if (campaignType == "all_fixedpriceshipping")
            {
                return new Campaign()
                {
                    ID = 4,
                    Name = "100 TL Üzeri Alýþveriþlerde Kargo Sadece 1 TL",
                    Description = "100 TL üzeri alýþveriþlerinizde kargo bedeli sadece 1 TL",
                    Scope = CampaignScope.Basket,
                    DiscountType = DiscountType.FixedPriceShipping,
                    Discount = discount,
                    MinimumAmount = minAmount,
                    MinimumQuantity = minQuantity,
                    PublishDate = DateTime.Now,
                    ExpireDays = 7,
                    Status = Statuses.Active,
                };
            }
            return null;
        }
    }
}