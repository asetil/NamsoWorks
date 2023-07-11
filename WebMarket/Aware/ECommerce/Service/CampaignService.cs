using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aware.Authenticate.Model;
using Aware.Data;
using Aware.Dependency;
using Aware.ECommerce.Interface;
using Aware.ECommerce.Model;
using Aware.ECommerce.Util;
using Aware.Mail;
using Aware.Util;
using Aware.Util.Model;
using Aware.Util.Enums;
using Aware.ECommerce.Enums;
using Aware.Authenticate;
using Aware.ECommerce.Model.Custom;
using Aware.Util.Lookup;

namespace Aware.ECommerce.Service
{
    public class CampaignService : ICampaignService
    {
        private readonly IRepository<Campaign> _campaignRepository;
        private readonly IRepository<Discount> _discountRepository;
        private readonly IRepository<User> _userRepository;

        private readonly ISessionManager _sessionManager;
        private readonly IMailService _mailService;
        private readonly ICategoryService _categoryService;
        private readonly IApplication _application;


        public CampaignService(ISessionManager sessionManager,IRepository<Campaign> campaignRepository, IRepository<Discount> discountRepository, IRepository<User> userRepository, IMailService mailService, ICategoryService categoryService, IApplication application)
        {
            _sessionManager = sessionManager;
            _mailService = mailService;
            _categoryService = categoryService;
            _application = application;
            _campaignRepository = campaignRepository;
            _discountRepository = discountRepository;
            _userRepository = userRepository;
        }

        public CampaignListModel GetManagerCampaigns(int managerID)
        {
            try
            {
                List<Campaign> campaignList = null;
                if (managerID > 0)
                {
                    campaignList = _campaignRepository.Where(i => i.OwnerID == managerID).ToList();
                }
                else
                {
                    campaignList = _campaignRepository.GetAll();
                }

                var result=new CampaignListModel()
                {
                    CampaignList = campaignList,
                    ScopeList = _application.Lookup.GetLookups(LookupType.CampaignScopes),
                    DiscountTypeList = _application.Lookup.GetLookups(LookupType.DiscountTypes)
                };
                return result;
            }
            catch (Exception ex)
            {
                _application.Log.Error("CampaignService > GetManagerCampaigns - failed for managerID:{0}",ex,managerID);
            }
            return null;
        }

        public Campaign GetAdminCampaign(int campaignID, int ownerID)
        {
            if (ownerID > 0)
            {
                return _campaignRepository.Where(i => i.OwnerID == ownerID && i.ID == campaignID).First();
            }
            return _campaignRepository.Get(campaignID);
        }

        public List<Campaign> GetActiveCampaigns()
        {
            var cacheKey = string.Format(Constants.CK_ActiveCampaigns, _sessionManager.GetCurrentRegion());
            var result = _application.Cacher.Get<List<Campaign>>(cacheKey);
            if (result == null)
            {
                var storeService = WindsorBootstrapper.Resolve<IStoreService>();
                var storeIDs = storeService.GetRegionStores().Select(i => i.ID).ToList();
                result = _campaignRepository.Where(i => i.Status == Statuses.Active && i.PublishDate <= DateTime.Now).ToList()
                            .Where(i => i.PublishDate.AddDays(i.ExpireDays) >= DateTime.Now && i.HasStore(storeIDs))
                            .ToList();
                _application.Cacher.Add(cacheKey, result, 720);
            }
            return result;
        }

        public Campaign GetActiveCampaign(int campaignID)
        {
            if (campaignID > 0)
            {
                var result = GetActiveCampaigns();
                return result.FirstOrDefault(i => i.ID == campaignID);
            }
            return null;
        }

        public IEnumerable<Discount> GetUsedCoupons(int basketID)
        {
            if (basketID > 0)
            {
                return _discountRepository.Where(i => i.BasketID == basketID && i.IsUsed == 1 
                    && i.ExpireDate >= DateTime.Now && (i.DiscountType == DiscountType.CouponAsRate || i.DiscountType == DiscountType.CouponAsAmount)).ToList();
            }
            return null;
        }

        public List<Discount> GetBasketDiscounts(int basketID)
        {
            if (basketID > 0)
            {
                return _discountRepository.Where(i => i.BasketID == basketID && i.Status == Statuses.Active).ToList();
            }
            return null;
        }

        public List<Discount> CalculateBasketDiscounts(Basket basket)
        {
            var result = new List<Discount>();
            if (basket == null) { return result; }

            if (basket.Status == Statuses.OrderedBasket)
            {
                return GetBasketDiscounts(basket.ID);
            }

            var campaigns = GetActiveCampaigns();
            foreach (var campaign in campaigns)
            {
                //1. Sepete özgü kampanya faydalanma
                if (IsValidForBasket(campaign, basket))
                {
                    decimal discount;
                    string description;

                    switch (campaign.DiscountType)
                    {
                        case DiscountType.Rate:
                            discount = Math.Round(basket.Total * campaign.Discount / 100, 2);
                            description = string.Format("{0} TL X {1}%", basket.Total, campaign.Discount);
                            AddDiscount(ref result, campaign, basket, discount, description);
                            break;
                        case DiscountType.Amount:
                            discount = Math.Round(campaign.Discount, 2);
                            description = string.Format("{0} TL indirim", campaign.Discount);
                            AddDiscount(ref result, campaign, basket, discount, description);
                            break;
                        case DiscountType.Shipping:
                            discount = 0;
                            description = string.Format("{0}", campaign.Discount == 100 ? "Ücretsiz Kargo" : "%" + campaign.Discount + " kargo indirimi");
                            AddDiscount(ref result, campaign, basket, discount, description);
                            break;
                        case DiscountType.FixedPriceShipping:
                            discount = 0;
                            description = string.Format("Kargo sadece {0}", campaign.Discount.ToPrice());
                            AddDiscount(ref result, campaign, basket, discount, description);
                            break;
                        case DiscountType.CouponAsAmount:
                            discount = 0;
                            description = string.Format("{0} değerinde Hediye Çeki", campaign.Discount.ToPrice());
                            AddDiscount(ref result, campaign, basket, discount, description);
                            break;
                        case DiscountType.CouponAsRate:
                            discount = 0;
                            description = string.Format("%{0} indirimli Hediye Çeki", campaign.Discount.DecString());
                            AddDiscount(ref result, campaign, basket, discount, description);
                            break;
                    }
                }
            }

            //Kullanılmış hediye kuponu varsa sepete yansıtalım
            var usedDiscounts = GetUsedCoupons(basket.ID);
            if (usedDiscounts != null && usedDiscounts.Any())
            {
                foreach (var discount in usedDiscounts)
                {
                    var campaign = discount.AsCampaign();
                    var description = campaign.DiscountType == DiscountType.Rate ?
                            string.Format("{0} TL X {1}%", basket.Total, campaign.Discount) :
                            string.Format("{0} TL indirim", campaign.Discount);
                    var amount = discount.DiscountType == DiscountType.CouponAsAmount ? discount.Factor : Math.Round(basket.Total * discount.Factor / 100, 2);
                    AddDiscount(ref result, campaign, basket, amount, description);
                }
            }
            return result;
        }

        private Discount AddDiscount(ref List<Discount> list, Campaign campaign, Basket basket, decimal amount, string description)
        {
            if (campaign == null || basket == null) { return null; }
            var discount = new Discount()
            {
                DiscountType = campaign.DiscountType,
                CampaignID = campaign.ID,
                BasketID = basket.ID,
                Name = campaign.Name,
                Description = description,
                DateCreated = DateTime.Now,
                Status = Statuses.Active,
                Amount = basket.GrossTotal,
                Factor = campaign.Discount,
                Total = amount,
                IsUsed = 0,
                ExpireDate = campaign.PublishDate.AddDays(campaign.ExpireDays),
                UserID = basket.UserID
            };
            list.Add(discount);
            return discount;
        }

        private bool IsValidForBasket(Campaign campaign, Basket basket)
        {
            if (campaign == null || basket == null || campaign.IsExpired || basket.Items == null) { return false; }
            if (campaign.Scope == CampaignScope.Basket && basket.Items.Any())
            {
                var filters = campaign.GetFilters();
                if (filters != null && filters.Any())
                {
                    if (basket.StoreID > 0 && filters.ContainsKey("sid")) //Check Store
                    {
                        var storeIDs = filters["sid"];
                        if (!storeIDs.Contains(basket.StoreID)) { return false; }
                    }

                    var items = basket.Items.Where(i => i.Status == Statuses.Active);
                    if (filters.ContainsKey("cid")) //Check Category
                    {
                        var categoryIDs = filters["cid"];
                        var relatedCategories = _categoryService.GetRelatedCategoryIDs(categoryIDs);
                        if (relatedCategories != null && relatedCategories.Any())
                        {
                            items = items.Where(i => relatedCategories.Contains(i.Product.CategoryID)).ToList();                            
                        }
                    }

                    if (filters.ContainsKey("pid")) //Check Properties
                    {
                        var propertyIDs = filters["pid"];
                        items = items.Where(i =>
                        {
                            var productPropertyIDs = i.Product.Properties.Select(p => p.ID);
                            return propertyIDs.Intersect(productPropertyIDs).Any();
                        }).ToList();
                    }

                    var total = items.Sum(i => i.GrossTotal);
                    return total > campaign.MinimumAmount && items.Count() > campaign.MinimumQuantity;

                }
                return basket.Total > campaign.MinimumAmount && basket.Items.Count > campaign.MinimumQuantity;
            }
            return false;
        }

        public Result Save(Campaign model, int ownerID, HttpRequestBase request)
        {
            try
            {
                if (model == null) { return Result.Error(); }
                model.DateModified = DateTime.Now;
                if (model.ID > 0)
                {
                    var campaign = _campaignRepository.Get(model.ID);
                    if (campaign != null)
                    {
                        Mapper.Map(ref campaign, model);
                        _campaignRepository.Update(campaign);
                        model = campaign;
                    }
                }
                else
                {
                    model.DateCreated = DateTime.Now;
                    model.OwnerID = ownerID;
                    _campaignRepository.Add(model);
                }
                if (model == null) { throw new Exception("campaign model is not valid!"); }
                return Result.Success(model, Resource.General_Success);
            }
            catch (Exception ex)
            {
                _application.Log.Error("CampaignService > Save - Fail for ID:{0}", ex, model.ID);
            }
            return Result.Error();
        }

        public Result DeleteCampaign(int campaignID)
        {
            try
            {
                if (campaignID > 0)
                {
                    var campaign = _campaignRepository.Get(campaignID);
                    if (campaign != null)
                    {
                        campaign.Status = Statuses.Deleted;
                        campaign.DateModified = DateTime.Now;
                        _campaignRepository.Update(campaign);
                        return Result.Success(null, Resource.General_Success);
                    }
                }
            }
            catch (Exception ex)
            {
                _application.Log.Error(string.Format("{0} ID'li kampanya silinirken hata oluştu", campaignID), ex);
            }
            return Result.Error(Resource.General_Error);
        }

        public Result UseCoupon(int userID, int basketID, string code)
        {
            try
            {
                if (userID > 0 && basketID > 0 && !string.IsNullOrEmpty(code))
                {
                    var coupons = _discountRepository.Where(i => i.Code == code && (i.UserID == userID || i.UserID == 0) && i.Status == Statuses.Active).ToList();
                    if (coupons.Any(i => i.IsUsed == 1))
                    {
                        return Result.Error(Resource.Campaign_CouponCodeIsAlreadyUsed);
                    }

                    if (coupons.Count() == 1)
                    {
                        var coupon = coupons.FirstOrDefault();
                        if (coupon.ExpireDate < DateTime.Now)
                        {
                            return Result.Error(Resource.Campaign_CouponCodeIsExpired);
                        }

                        //Set coupon as used
                        if (coupon.UserID > 0)
                        {
                            coupon.IsUsed = 1;
                            _discountRepository.Update(coupon, false);
                        }

                        //Add new discount for user basket
                        var usedCoupon = coupon.Clone(userID, 1);
                        usedCoupon.BasketID = basketID;
                        usedCoupon.Code = null;
                        usedCoupon.Name = "Hediye Kuponu İndirimi";
                        _discountRepository.Add(usedCoupon);
                        return Result.Success(null, Resource.Campaign_CouponCodeAppliedSuccessfully);
                    }
                }
            }
            catch (Exception ex)
            {
                _application.Log.Error("BasketService > UseCoupon - Fail with userID:{0}, basketID:{1}, code:{2}", ex, userID, basketID, code);
            }
            return Result.Error(Resource.Campaign_CouponCodeIsNotValid);
        }

        public bool SaveDiscounts(Basket basket, Order order)
        {
            try
            {
                if (basket != null && order != null && basket.Discounts != null && basket.Discounts.Any())
                {
                    var user = _userRepository.Get(basket.UserID);
                    var discounts = basket.Discounts.Where(i => i.ID == 0);
                    foreach (var discount in discounts)
                    {
                        if (discount.DiscountType == DiscountType.CouponAsRate || discount.DiscountType == DiscountType.CouponAsAmount)
                        {
                            discount.Code = GenerateCouponCode();
                            var description = string.Format("#{0} Nolu siparişinizle birlikte '{1}' kampanyasından faydalanma fırsatı yakaladınız.", order.UniqueID, discount.Name);
                            _mailService.SendCouponMail(user.Email, user.Name, description, discount.ExpireDate.ToString("d"), discount.Code);
                        }
                        _discountRepository.Add(discount);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                _application.Log.Error("CampaignService > SaveDiscounts - Fail for basket:{0},order:{1}", ex, basket.ID, order.ID);
            }
            return false;
        }

        private string GenerateCouponCode(int length = 8)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, length)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            return result;
        }
    }
}