using System.Collections.Generic;
using System.Linq;
using Aware.ECommerce.Model;
using System;
using Aware.Cache;
using Aware.Data;
using Aware.ECommerce.Interface;
using Aware.ECommerce.Util;
using Aware.Util;
using Aware.Util.Log;
using Aware.Util.Model;
using Aware.ECommerce.Enums;
using Aware.Util.Enums;

namespace Aware.ECommerce.Service
{
    public class BasketService : IBasketService
    {
        private readonly object _lock = new object();
        private readonly IRepository<Basket> _basketRepository;
        private readonly IRepository<BasketItem> _basketItemRepository;
        private readonly IRepository<StoreItem> _itemRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly ICampaignService _campaignService;
        private readonly IStoreService _storeService;
        private readonly IVariantService _variantService;
        private readonly ICacher _cacher;
        private readonly ILogger _logger;

        public BasketService(ICampaignService campaignService, ICacher cacher, IRepository<Basket> basketRepository,
            IRepository<BasketItem> basketItemRepository, IRepository<StoreItem> itemRepository, IRepository<Product> productRepository, IStoreService storeService, IVariantService variantService, ILogger logger)
        {
            _campaignService = campaignService;
            _cacher = cacher;
            _basketRepository = basketRepository;
            _basketItemRepository = basketItemRepository;
            _itemRepository = itemRepository;
            _productRepository = productRepository;
            _storeService = storeService;
            _variantService = variantService;
            _logger = logger;
        }

        public Basket GetUserBasket(int userID, int basketID = 0, bool createEnabled = false)
        {
            Basket basket = null;
            try
            {
                if (basketID > 0 && userID > 0)
                {
                    basket = _basketRepository.Where(i => i.ID == basketID && i.UserID == userID).First();
                }
                else if (basketID > 0)
                {
                    basket = _basketRepository.Where(i => i.ID == basketID).First();
                }
                else
                {
                    basket = _basketRepository.Where(i => i.UserID == userID && i.Status == Statuses.Active).First();
                }
                if (createEnabled && (basket == null || basket.ID == 0)) { basket = CreateBasket(userID); }
            }
            catch (Exception ex)
            {
                _logger.Error("BasketService > GetUserBasket - Fail for userID:{0} - {1} - {2}", ex, userID, basketID, createEnabled);
            }
            return basket ?? EmptyBasket;
        }

        public Basket GetBasketWithDiscounts(int userID, int basketID)
        {
            try
            {
                var basket = GetUserBasket(userID, basketID);
                if (basket != null && basket.ID > 0)
                {
                    basket.Items = _basketItemRepository.Where(i => i.BasketID == basket.ID && i.Status == Statuses.Active).ToList();
                    if (basket.Items != null && basket.Items.Any())
                    {
                        var productIDs = basket.Items.Select(i => i.ProductID).Distinct();
                        var products = _productRepository.Where(p => productIDs.Contains(p.ID)).ToList();
                        basket.Items = basket.Items.Select(bi =>
                        {
                            bi.Product = products.FirstOrDefault(p => p.ID == bi.ProductID);
                            return bi;
                        }).ToList();
                    }

                    basket.Discounts = _campaignService.CalculateBasketDiscounts(basket);
                }
                return basket;
            }
            catch (Exception ex)
            {
                _logger.Error("BasketService > GetBasketWithDiscounts - Fail for userID:{0} - {1}", ex, userID, basketID);
            }
            return EmptyBasket;
        }

        public Basket GetBasketSummary(int userID, int basketID)
        {
            try
            {
                var key = string.Format(Util.Constants.CK_BasketSummary, userID);
                var basket = _cacher.Get<Basket>(key);
                if (basket == null)
                {
                    basket = GetBasketWithDiscounts(userID, basketID);
                    if (basket != null)
                    {
                        _cacher.Add(key, basket);
                    }
                }
                return basket;
            }
            catch (Exception ex)
            {
                _logger.Error("BasketService > GetBasketSummary - Fail for userID:{0} - {1}", ex, userID, basketID);
            }
            return EmptyBasket;
        }

        public Result CheckBasketBeforePurchase(int userID, Basket basket = null)
        {
            try
            {
                basket = basket ?? GetBasketWithDiscounts(userID, 0);
                if (basket == null || basket.ID == 0) { throw new Exception("Kullanıcı sepeti bulunamadı!!!"); }

                if (basket.Items == null || !basket.Items.Any())
                {
                    return Result.Error("Sepetinizde ürün bulunamadı!");
                }

                var store = _storeService.GetRegionStore(basket.StoreID);
                if (store == null || (store.MinOrderAmount > basket.Total))
                {
                    return Result.Error(string.Format(Resource.Order_StoreMinAmountNotPassed, store.MinOrderAmount.ToPrice()));
                }
                return Result.Success(null, Resource.General_Success);
            }
            catch (Exception ex)
            {
                _logger.Error("BasketService > CheckBasketBeforePurchase - Fail with userID:{0}", ex, userID);
            }
            return Result.Error(Resource.General_Error);
        }

        public Result AddItemToBasket(int userID, int itemID, decimal quantity, string variantSelection)
        {
            if (itemID <= 0 || quantity <= 0)
            {
                return Result.Error(Resource.Basket_ItemNotFound);
            }

            var item = _itemRepository.Get(itemID);
            return AddItemToBasket(userID, item, quantity, variantSelection);
        }

        public Result AddItemToBasket(int userID, StoreItem item, decimal quantity, string variantSelection)
        {
            lock (_lock)
            {
                var guid = _basketItemRepository.StartTransaction();
                try
                {
                    if (item == null) { return Result.Error(Resource.Basket_ItemNotFound); }
                    if (userID == 0) { return Result.Error(2,Resource.Basket_AddToBasketFailed); }

                    var basket = GetUserBasket(userID, 0, true);
                    if (basket == null || basket.ID == 0) { return Result.Error(Resource.Basket_AddToBasketFailed); }

                    var basketStore = _storeService.GetRegionStore(basket.StoreID);
                    if (basketStore != null && basketStore.ID > 0 && basketStore.ID != item.StoreID)
                    {
                        return Result.Error(Resource.Basket_SingleStoreAllowed);
                    }

                    quantity = (quantity == -1 ? 1 : quantity);
                    var variantCode = item.HasVariant ? _variantService.GetVariantCode(variantSelection) : 0;

                    var basketItem = _basketItemRepository.Where(i => i.ItemID == item.ID && i.Status == Statuses.Active && i.BasketID == basket.ID && i.VariantCode == variantCode).First();
                    if (basketItem == null || basketItem.ID <= 0)
                    {
                        basketItem = new BasketItem
                        {
                            BasketID = basket.ID,
                            ItemID = item.ID,
                            ProductID = item.ProductID,
                            StoreID = item.StoreID,
                            DateCreated = DateTime.Now,
                        };
                    }

                    var checkResult = CheckItemBeforeAdd(item, quantity, basketItem.Quantity, variantSelection);
                    if (!checkResult.Valid) { return Result.Error(checkResult.Message, checkResult); }

                    if (item.HasVariant)
                    {
                        basketItem.VariantPrice = checkResult.Price;
                        basketItem.VariantDescription = checkResult.Description;
                    }

                    basketItem.VariantCode = variantCode;
                    basketItem.Quantity += quantity;
                    basketItem.Price = item.SalesPrice;
                    basketItem.ListPrice = item.ListPrice;
                    basketItem.GrossTotal = basketItem.Quantity * (basketItem.Price + basketItem.VariantPrice);
                    basketItem.Status = Statuses.Active;
                    basketItem.DateModified = DateTime.Now;

                    if (basketItem.ID == 0) { _basketItemRepository.Add(basketItem); }
                    _basketItemRepository.Save();

                    if (item.HasVariant)
                    {
                        var basketItemVariant = new VariantSelection
                        {
                            RelationID = basketItem.ID,
                            RelationType = (int)RelationTypes.BasketItem,
                            VariantCombination = variantSelection,
                            Price = basketItem.VariantPrice,
                            Code = basketItem.VariantCode,
                        };
                        _variantService.SaveVariantSelection(basketItemVariant);
                    }

                    if (item.HasVariant && checkResult.UseStock)
                    {
                        _variantService.RefreshVariantStock(checkResult.VariantSelection, quantity);
                    }
                    else
                    {
                        RefreshItemStock(item.ID, quantity);
                    }

                    ClearBasketCache(userID);
                    _basketItemRepository.Commit(guid);
                    return Result.Success(basket.ID, Resource.General_Success);
                }
                catch (Exception ex)
                {
                    _logger.Error(string.Format("{0} ID'li kullanıcının sepetine ürün eklenirken hata!!!", userID), ex);
                    _basketItemRepository.Rollback(guid);
                }
                return Result.Error(Resource.Basket_AddToBasketFailed);
            }
        }

        public Result ChangeBasketItemQuantity(int userID, int basketID, int basketItemID, decimal quantity)
        {
            var guid = _basketItemRepository.StartTransaction();
            try
            {
                var basketItem = _basketItemRepository.Where(i => i.ID == basketItemID && i.BasketID == basketID).First();
                if (basketItem != null && basketItem.ID > 0)
                {
                    var item = _itemRepository.Get(basketItem.ItemID) ?? new StoreItem();
                    var refreshQuantity = quantity - basketItem.Quantity;

                    VariantSelection itemVariantSelection = null;
                    if (basketItem.VariantCode > 0)
                    {
                        var variantSelection = _variantService.GetVariantSelection(basketItem.ID, (int)RelationTypes.BasketItem, basketItem.VariantCode);
                        if (variantSelection != null)
                        {
                            var checkResult = _variantService.CheckVariantSelection(variantSelection.VariantCombination, basketItem.ItemID,(int)RelationTypes.StoreItem);
                            if (checkResult.UseStock)
                            {
                                itemVariantSelection = checkResult.VariantSelection;
                            }
                        }
                    }

                    var hasStock=(itemVariantSelection!=null && itemVariantSelection.HasStock(refreshQuantity) || (itemVariantSelection==null && item.HasStock(refreshQuantity)));
                    if (!hasStock)
                    {
                        return Result.Error(Resource.Basket_NotEnoughStock);
                    }

                    if (quantity > 20)
                    {
                        return Result.Error(Resource.Basket_QuantityLimitExceeded);
                    }

                    basketItem.Quantity = quantity;
                    basketItem.GrossTotal = quantity * (basketItem.Price + basketItem.VariantPrice);
                    basketItem.DateModified = DateTime.Now;
                    _basketItemRepository.Update(basketItem);

                    if (item.HasVariant && itemVariantSelection!=null)
                    {
                        _variantService.RefreshVariantStock(itemVariantSelection, refreshQuantity);
                    }
                    else
                    {
                        RefreshItemStock(basketItem.ItemID, refreshQuantity);
                    }
                    
                    ClearBasketCache(userID);
                    _basketItemRepository.Commit(guid);
                    return Result.Success(basketItem, Resource.General_Success);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("BasketService > ChangeBasketItemQuantity - Fail with basketItemID:{0}, basketID:{1}", ex, basketItemID, basketID);
                _basketItemRepository.Rollback(guid);
            }
            return Result.Error(Resource.General_Error);
        }

        public Result DeleteBasketItem(int userID, int basketID, int basketItemID)
        {
            var guid = _basketItemRepository.StartTransaction();
            try
            {
                if (basketID > 0 && basketItemID > 0)
                {
                    var item = _basketItemRepository.Where(i => i.ID == basketItemID && i.BasketID == basketID).First();
                    if (item != null && item.ID > 0)
                    {
                        bool isDeleted = _basketItemRepository.Delete(basketItemID);
                        if (isDeleted)
                        {
                            var quantity = -1 * item.Quantity;
                            var refreshItemStock = true;

                            if (item.VariantCode > 0)
                            {
                                var variantSelection=_variantService.GetVariantSelection(item.ID, (int)RelationTypes.BasketItem, item.VariantCode);
                                if (variantSelection != null)
                                {
                                    var itemVariantSelection=_variantService.CheckVariantSelection(variantSelection.VariantCombination,item.ItemID, (int)RelationTypes.StoreItem);
                                    if (itemVariantSelection.UseStock)
                                    {
                                        refreshItemStock = false;
                                        _variantService.RefreshVariantStock(itemVariantSelection.VariantSelection, quantity);
                                    }
                                }
                            }

                            if (refreshItemStock)
                            {
                                RefreshItemStock(item.ItemID, quantity);
                            }

                            ClearBasketCache(userID);
                            _basketItemRepository.Commit(guid);
                            return Result.Success(null, Resource.Basket_ItemDeletedSuccessfully);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("BasketService > DeleteBasketItem - Fail with basketItemID:{1}, basketID:{2}", ex, basketItemID, basketID);
                _basketItemRepository.Rollback(guid);
            }
            return Result.Error(Resource.Basket_ItemCannotBeDeleted);
        }

        private bool RefreshItemStock(int itemID, decimal quantity)
        {
            var item = _itemRepository.Get(itemID);
            return RefreshItemStock(item, quantity);
        }

        private bool RefreshItemStock(StoreItem item, decimal quantity)
        {
            try
            {
                if (item != null && item.ID > 0)
                {
                    if (item.Stock == -1)
                    {
                        return true;
                    }

                    var newStock = item.Stock - quantity;
                    if (newStock < 0) { throw new Exception(); }

                    item.Stock = newStock;
                    _itemRepository.Update(item);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("BasketService > RefreshItemStock - Fail with : {0}!", ex, item.ID);
                throw ex;
            }
            return false;
        }

        public Basket CreateBasket(int userID, string name = "", Statuses status = Statuses.Active)
        {
            if (userID > 0)
            {
                Basket basket = new Basket
                {
                    UserID = userID,
                    Name = name,
                    Status = status,
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now,
                };

                _basketRepository.Add(basket);
                return _basketRepository.Where(i => i.UserID == userID && i.Status == status).ToList().LastOrDefault();
            }
            return null;
        }

        public Result ClearBasket(int userID, int basketID, bool deleteBasket)
        {
            try
            {
                var sql = SqlHelper.ClearBasket(userID, basketID, deleteBasket);
                var result = _basketRepository.ExecuteSp(sql);
                if (result != 0) //TODO# : Bu koşul >0 olmalı sanki
                {
                    ClearBasketCache(userID);
                    return Result.Success(null, Resource.General_Success);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("BasketService > ClearBasket - Fail for userID:{0}, basketID:{1}", ex, userID, basketID);
            }
            return Result.Error(Resource.General_Error);
        }

        public bool SetBasketOrdered(Order order)
        {
            if (order != null && order.BasketID > 0)
            {
                var basket = GetBasketWithDiscounts(order.UserID, order.BasketID);
                if (basket != null && basket.ID > 0)
                {
                    if (basket.Status != Statuses.OrderedBasket)
                    {
                        _campaignService.SaveDiscounts(basket, order);
                        basket.Status = Statuses.OrderedBasket;
                        basket.DateModified = DateTime.Now;

                        var store = _storeService.GetRegionStore(basket.StoreID);
                        basket.Name = store.DisplayName;
                        _basketRepository.Update(basket);
                    }
                    return true;
                }
            }
            return false;
        }

        public Result AddFavoritesToBasket(int userID, int storeID, string productIDs)
        {
            try
            {
                //TODO#2: Can use transaction here
                if (userID > 0 && storeID > 0 && !string.IsNullOrEmpty(productIDs.Trim(',')))
                {
                    var productIDList = productIDs.Trim(',').Split(',').Select(a => a.Int() as object).ToList();
                    var criteriaHelper = _itemRepository.CriteriaHelper
                                     .WithAlias("Product", "pr")
                                     .Eq("StoreID", storeID)
                                     .Eq("Status", Statuses.Active)
                                     .In("ProductID", productIDList.ToArray());

                    var items = _itemRepository.GetWithCriteria(criteriaHelper);
                    if (items != null && items.Any())
                    {
                        var failCount = 0;
                        var messages = new List<string>();

                        //Clear invalid basket items
                        var basket = GetUserBasket(userID);
                        var basketItems = _basketItemRepository.Where(i => i.BasketID == basket.ID && i.Status == Statuses.Active && i.StoreID != storeID).ToList();
                        if (basketItems != null && basketItems.Any())
                        {
                            foreach (var basketItem in basketItems)
                            {
                                _basketItemRepository.Delete(basketItem.ID);
                            }
                            _basketItemRepository.Save();
                        }

                        foreach (var item in items)
                        {
                            var result = AddItemToBasket(userID, item, item.Product.UnitFactor, string.Empty);
                            messages.Add(string.Format("{0} - {1}", item.Product.Name, result.OK ? "Başarıyla Eklendi" : result.Message));
                            if (!result.OK) { failCount++; }
                        }

                        if (failCount == productIDList.Count())
                        {
                            return Result.Error(Resource.Basket_AddFavoritesToBasketFailed, messages);
                        }
                        return Result.Success(failCount > 0 ? messages : null, failCount > 0 ? Resource.Basket_AddFavoritesToBasketPartiallyFailed : Resource.Basket_AddFavoritesToBasketSuccessfull);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("BasketService > OrderList - Fail with userID:{0}, storeID:{1}", ex, userID, storeID);
            }
            return Result.Error(Resource.Basket_AddFavoritesToBasketFailed);
        }

        public Result UseCoupon(int userID, int basketID, string code)
        {
            return _campaignService.UseCoupon(userID, basketID, code);
        }

        private VariantCheckResult CheckItemBeforeAdd(StoreItem item, decimal quantity, decimal oldQuantity, string variantSelection)
        {
            var result = new VariantCheckResult();
            if (item.HasVariant)
            {
                result = _variantService.CheckVariantSelection(variantSelection, item.ID, (int)RelationTypes.StoreItem);
                if (result.Valid)
                {
                    if (result.UseStock && result.VariantSelection.Stock != -1 && result.VariantSelection.Stock < quantity)
                    {
                        result.Message = Resource.Basket_NotEnoughStock;
                    }
                }
            }

            if (result.Valid && !result.UseStock && !item.HasStock(quantity))
            {
                result.Message = Resource.Basket_NotEnoughStock;
            }

            var maxQuantity = item.Product.Unit == MeasureUnits.Gram ? 1000 : 10;
            if (result.Valid && (quantity + oldQuantity) > maxQuantity)
            {
                result.Message = string.Format(Resource.Basket_QuantityLimitExceeded, maxQuantity, item.Product.UnitDescription);
            }
            return result;
        }

        private Basket EmptyBasket
        {
            get
            {
                return new Basket
                {
                    Items = new List<BasketItem>()
                };
            }
        }

        private void ClearBasketCache(int userID)
        {
            if (userID > 0)
            {
                var key = string.Format(Constants.CK_BasketSummary, userID);
                _cacher.Remove(key);
            }
        }
    }
}