using System.Collections.Generic;
using System.Linq;
using Aware.ECommerce.Model;
using System;
using Aware.Authenticate.Model;
using Aware.Data;
using Aware.ECommerce.Interface;
using Aware.ECommerce.Util;
using Aware.Util;
using Aware.Dependency;
using Aware.ECommerce.Search;
using Aware.Regional;
using Aware.Regional.Model;
using Aware.Util.Model;
using Aware.ECommerce.Enums;
using Aware.Util.Enums;
using Aware.Util.Lookup;

namespace Aware.ECommerce.Service
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<ShippingMethod> _shippingMethodRepository;

        private readonly IBasketService _basketService;
        private readonly IAddressService _addressService;
        private readonly IStoreService _storeService;
        private readonly IApplication _application;

        public OrderService(IBasketService basketService, IAddressService addressService,
            IRepository<Order> orderRepository, IRepository<User> userRepository, IRepository<ShippingMethod> shippingMethodRepository,
            IApplication application, IStoreService storeService)
        {
            _basketService = basketService;
            _addressService = addressService;
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _shippingMethodRepository = shippingMethodRepository;
            _application = application;
            _storeService = storeService;
        }

        public OrderSearchResult SearchOrders(OrderSearchParams searchParams, int customerID)
        {
            try
            {
                var storeList = _storeService.GetCustomerStores(customerID);
                if (storeList == null || !storeList.Any())
                {
                    throw new Exception("User has no stores associated!");
                }

                searchParams = searchParams ?? OrderSearchParams.Init();
                if ((searchParams.StoreIDs == null || !searchParams.StoreIDs.Any()))
                {
                    searchParams.StoreIDs = storeList.Select(i => i.ID);
                }

                var searchResult = _orderRepository.Find(searchParams);
                var userIDList = searchResult.Results.Select(u => u.UserID);
                var users = _userRepository.Where(i => userIDList.Contains(i.ID)).ToList();

                var result = new OrderSearchResult
                {
                    Success = true,
                    TotalSize = searchResult.TotalSize,
                    SearchParams = searchParams,
                    StoreList = storeList,
                    OrderStatusList = _application.Lookup.GetLookups(LookupType.OrderStatus),
                    PaymentTypes = _application.Lookup.GetLookups(LookupType.PaymentTypes),
                    Results = searchResult.Results.Select(i =>
                    {
                        i.OrderDetail = new OrderDetail()
                        {
                            User = users.FirstOrDefault(u => u.ID == i.UserID),
                            Store = storeList.FirstOrDefault(s => s.ID == i.StoreID)
                        };
                        return i;
                    }).ToList()
                };
                return result;
            }
            catch (Exception ex)
            {
                //TODO# : Burda filter json olarak eklenebilir loga
                _application.Log.Error("OrderService > SearchOrders - Fail for params:{0}", ex, "TODO");
            }
            return null;
        }

        public OrderViewModel GetOrderViewModel(int orderID, int userID, OrderStatuses status = 0, bool loadUserInfo = false)
        {
            Order order = null;
            if (orderID > 0 && userID > 0)
            {
                order = _orderRepository.Where(i => i.ID == orderID && i.UserID == userID).First();
            }
            else if (orderID > 0 && userID == -1) //for admin side
            {
                order = _orderRepository.Get(orderID);
            }
            else if (userID > 0 && status > 0)
            {
                order = _orderRepository.Where(i => i.UserID == userID && i.Status == status).ToList().LastOrDefault();
            }

            if (order != null && order.ID > 0)
            {
                var user = loadUserInfo ? _userRepository.Get(order.UserID) : new User();
                var basket = _basketService.GetBasketWithDiscounts(userID, order.BasketID);
                var paymentTypeDesc = _application.Lookup.GetLookupName(LookupType.PaymentTypes, order.PaymentType);

                return new OrderViewModel()
                {
                    Order = order,
                    Basket = basket,
                    ShippingMethods = GetOrderShippingMethods(order.ShippingMethodID, basket),
                    AddressList = _addressService.GetOrderAddresses(order),
                    User = user,
                    Settings = _application.Order,
                    PaymentTypeDesc = paymentTypeDesc
                };
            }
            return null;
        }

        public List<Order> GetUserOrders(int userID)
        {
            var orderList = _orderRepository.Where(i => i.UserID == userID && i.Status > OrderStatuses.WaitingCustomerApproval)
                .SortBy(o => o.DateCreated, true).ToList();
            return orderList;
        }

        public Order GetUserOrder(int userID,int orderID)
        {
            if (userID > 0 && orderID > 0)
            {
                return _orderRepository.First(i => i.UserID == userID && i.ID == orderID);;
            }
            return null;
        }

        public Result Approve(int userID, int orderID)
        {
            var transactionGuid = Guid.Empty;
            try
            {
                if (orderID > 0)
                {
                    transactionGuid = _orderRepository.StartTransaction();
                    var order = _orderRepository.Where(i => i.UserID == userID && i.ID == orderID && i.Status == OrderStatuses.WaitingCustomerApproval).First();
                    if (order != null && _basketService.SetBasketOrdered(order))
                    {
                        order.Status = OrderStatuses.WaitingApproval;
                        order.DateModified = DateTime.Now;
                        _orderRepository.Update(order);
                        _orderRepository.Commit(transactionGuid);
                        return Result.Success(order.UniqueID, Resource.General_Success);
                    }
                }
            }
            catch (Exception ex)
            {
                _application.Log.Error("OrderService > Approve - Fail for userID:{0}, orderID:{1}!", ex, userID, orderID);
                if (transactionGuid != Guid.Empty) { _orderRepository.Rollback(transactionGuid); }
            }
            return Result.Error(Resource.Order_OrderCannotBeApproved);
        }

        public Result SavePaymentInfo(int userID, int orderID, PaymentType paymentType, int subPaymentType, int installment, out bool savedBefore)
        {
            savedBefore = false;
            var transactionGuid = Guid.Empty;

            try
            {
                if (orderID > 0 && paymentType != PaymentType.None)
                {
                    var payment = _application.Order.GetPayment(paymentType);
                    if (payment == null || (paymentType == PaymentType.CreditCard && subPaymentType <= 0))
                    {
                        return Result.Error(Resource.Order_PaymentInfoCannotBeSaved);
                    }

                    if (paymentType == PaymentType.Remittance && subPaymentType <= 0)
                    {
                        return Result.Error(Resource.Order_RemittanceBankNotSelected);
                    }

                    transactionGuid = _orderRepository.StartTransaction();
                    var order = _orderRepository.Where(i => i.UserID == userID && i.ID == orderID
                                                            && (i.Status == OrderStatuses.WaitingCustomerApproval || i.Status == OrderStatuses.WaitingPayment)).First();

                    if (order != null && _basketService.SetBasketOrdered(order))
                    {
                        savedBefore = order.Status != OrderStatuses.WaitingCustomerApproval;
                        order.Status = OrderStatuses.WaitingApproval;
                        if (paymentType == PaymentType.CreditCard || paymentType == PaymentType.Remittance || paymentType == PaymentType.GarantiPay)
                        {
                            order.Status = OrderStatuses.WaitingPayment;
                        }

                        order.PaymentType = (int)paymentType;
                        order.SubPaymentType = subPaymentType;
                        order.PaymentCost = payment.Cost;
                        order.Installment = installment;
                        order.GrossTotal = order.BasketTotal + order.DiscountTotal + order.ShipmentCost;

                        if (paymentType == PaymentType.CreditCard && installment >= 2)
                        {
                            var paymentService = WindsorBootstrapper.Resolve<IPaymentService>();
                            var installmentInfo = paymentService.GetCachedInstallments(subPaymentType).FirstOrDefault(i => i.Count == installment);
                            if (installmentInfo != null && installmentInfo.Commission != 0)
                            {
                                order.PaymentCost += (order.GrossTotal * installmentInfo.Commission / 100);
                            }
                        }

                        order.GrossTotal += order.PaymentCost;
                        order.DateModified = DateTime.Now;

                        _orderRepository.Update(order);
                        _orderRepository.Commit(transactionGuid);
                        return Result.Success(order, Resource.General_Success);
                    }
                }
            }
            catch (Exception ex)
            {
                _application.Log.Error("OrderService > SavePaymentInfo - Fail for userID:{0}, orderID:{1}, paymentType:{2}!", ex, userID, orderID, paymentType);
                _orderRepository.Rollback(transactionGuid);
            }
            return Result.Error(Resource.Order_PaymentInfoCannotBeSaved);
        }

        public Result ChangeOrderStatus(int orderID, OrderStatuses status)
        {
            try
            {
                if (orderID > 0)
                {
                    var order = _orderRepository.Get(orderID);
                    if (order != null && order.ID > 0)
                    {
                        order.Status = status;
                        order.DateModified = DateTime.Now;
                        _orderRepository.Update(order);
                        return Result.Success(order, Resource.General_Success);
                    }
                }
            }
            catch (Exception ex)
            {
                _application.Log.Error("OrderService > ChangeOrderStatus - Fail for orderID:{0}, status:{1}!", ex, orderID, status);
            }
            return Result.Error(Resource.Order_OrderCannotBeEdited);
        }

        public Result CancelOrder(int userID, int orderID)
        {
            try
            {
                if (userID > 0 && orderID > 0)
                {
                    var order = _orderRepository.Where(i => i.UserID == userID && i.ID == orderID).First();
                    if (order != null && (order.Status == OrderStatuses.WaitingApproval || order.Status == OrderStatuses.WaitingPayment))
                    {
                        order.Status = OrderStatuses.CancelledOrder;
                        order.DateModified = DateTime.Now;
                        _orderRepository.Update(order);

                        _application.Log.Warn(string.Format("OrderService > User Cancelled Order - userID:{0}, orderID:{1}", userID, orderID), "CANCELLED ORDER");
                        return Result.Success(Resource.General_Success);
                    }
                    else if (order != null)
                    {
                        return Result.Error(Resource.Order_OrderCannotBeCancelled);
                    }
                }
            }
            catch (Exception ex)
            {
                _application.Log.Error("OrderService > Cancel Order - Fail with userID:{0}, orderID:{1}", ex, userID, orderID);
            }
            return Result.Error(Resource.General_Error);
        }

        public Result SaveOrder(Order model, int userID)
        {
            if (userID > 0 && model != null && model.ShippingAddressID > 0 && model.BillingAddressID > 0)
            {
                if (model.ID > 0)
                {
                    var order = _orderRepository.Where(i => i.ID == model.ID && i.UserID == userID).First();
                    if (order != null)
                    {
                        order.Note = model.Note;
                        order.ShippingAddressID = model.ShippingAddressID;
                        order.BillingAddressID = model.BillingAddressID;
                        order.ShippingMethodID = model.ShippingMethodID;
                        order.PaymentType = model.PaymentType;
                        order.SubPaymentType = model.SubPaymentType;
                        return SaveOrder(order);
                    }
                }
                else
                {
                    model.UserID = userID;
                    return SaveOrder(model);
                }
            }
            return Result.Error(Resource.Order_OrderCannotBeEdited);
        }

        public Result SaveOrder(Order model)
        {
            if (model == null) return Result.Error();
            try
            {
                var basket = _basketService.GetBasketWithDiscounts(model.UserID, model.BasketID);
                var checkResult = _basketService.CheckBasketBeforePurchase(model.UserID, basket);
                if (!checkResult.OK) { return checkResult; }

                var shippingPrice = GetShippingCost(model.ShippingMethodID, basket);
                var paymentInfo = _application.Order.GetPayment((PaymentType)model.PaymentType) ?? new SimpleItem();
                if (model.ID > 0)
                {
                    var order = _orderRepository.Get(model.ID);
                    if (order != null)
                    {
                        Mapper.Map(ref order, model);
                        Mapper.MapOrderTotals(ref order, basket, shippingPrice, paymentInfo.Cost);
                        _orderRepository.Update(order);
                        return Result.Success(order, Resource.General_Success);
                    }
                }
                else
                {
                    model.BasketID = basket.ID;
                    model.StoreID = basket.StoreID;
                    model.Status = OrderStatuses.WaitingCustomerApproval;
                    Mapper.MapOrderTotals(ref model, basket, shippingPrice, paymentInfo.Cost);
                    model.DateModified = DateTime.Now;
                    model.DateCreated = DateTime.Now;
                    _orderRepository.Add(model);
                    return Result.Success(model, Resource.General_Success);
                }
            }
            catch (Exception ex)
            {
                _application.Log.Error("OrderService > Edit Order - Failure for orderID:{0}", ex, model.ID);
            }
            return Result.Error(Resource.Order_OrderCannotBeEdited);
        }

        public Result GetRawOrder(int userID)
        {
            var basket = _basketService.GetBasketWithDiscounts(userID, 0);
            if (basket != null && basket.ID > 0)
            {
                var checkResult = _basketService.CheckBasketBeforePurchase(userID, basket);
                if (!checkResult.OK)
                {
                    return checkResult;
                }

                var order = _orderRepository.Where(i => i.UserID == userID && i.BasketID == basket.ID && i.Status == OrderStatuses.WaitingCustomerApproval).First();
                order = order ?? new Order();

                var addressList = _addressService.GetUserAddresses(userID, true).AddressList;
                var shippingMethods = GetOrderShippingMethods(0, basket, GetAddressRegions(addressList));

                var model = new OrderViewModel()
                {
                    Order = order,
                    Basket = basket,
                    AddressList = addressList,
                    ShippingMethods = shippingMethods,
                    SelectedShippingAddress = order.ShippingAddressID,
                    SelectedBillingAddress = order.BillingAddressID,
                    Settings = _application.Order,
                    AllowAddressSelection = true
                };
                return Result.Success(model);
            }
            return Result.Error(Resource.Order_NoBasketNoItem);
        }

        public ShippingMethod GetShippingMethod(int id)
        {
            if (id > 0)
            {
                var result = _shippingMethodRepository.Get(id);
                return result;
            }
            return null;
        }

        public List<ShippingMethod> GetAllShippingMethods()
        {
            return _shippingMethodRepository.GetAll();
        }

        public List<ShippingMethod> GetRegionShippingMethods(int userID, List<int> regionIDs)
        {
            if (userID > 0 && regionIDs != null && regionIDs.Any())
            {
                var allRegions = new List<int>();
                foreach (var regionID in regionIDs)
                {
                    var subRegionID = regionID;
                    while (subRegionID > 0)
                    {
                        allRegions.Add(subRegionID);
                        var region = _addressService.GetRegion(subRegionID);
                        subRegionID = region != null && region.ParentID > 0 ? region.ParentID : 0;
                    }
                }

                var userBasket = _basketService.GetBasketWithDiscounts(userID, 0);
                return GetOrderShippingMethods(0, userBasket, allRegions);
            }
            return null;
        }

        private List<ShippingMethod> GetOrderShippingMethods(int shippingMethod, Basket basket, List<int> regionIDs = null)
        {
            var result = _application.Cacher.Get<List<ShippingMethod>>(Constants.CK_ShippingMethods);
            if (result == null || !result.Any())
            {
                result = _shippingMethodRepository.Where(i => i.Status == Statuses.Active).ToList();
                _application.Cacher.Add(Constants.CK_ShippingMethods, result);
            }

            regionIDs = regionIDs ?? new List<int>();
            result = result.Where(i => i.ID == shippingMethod || regionIDs.Any(i.HasRegion) || i.HasRegion(-1)).ToList();

            if (result.Any() && basket != null)
            {
                result = result.Select(shipping =>
                {
                    if (basket.Discounts != null)
                    {
                        var discount = basket.Discounts.FirstOrDefault(i => i.DiscountType == DiscountType.FixedPriceShipping || i.DiscountType == DiscountType.Shipping);
                        if (discount != null)
                        {
                            var price = discount.DiscountType == DiscountType.FixedPriceShipping ? discount.Factor : shipping.Price * (100 - discount.Factor) / 100;
                            if (price < shipping.Price)
                            {
                                shipping.DiscountInfo = string.Format("{0}, {1} yerine {2} ({3})", shipping.Name, shipping.Price.ToPrice(), price.ToPrice(), discount.Description);
                                shipping.Price = price;
                            }
                        }
                    }
                    return shipping;
                }).ToList();
            }
            return result;
        }

        public Result SaveShippingMethod(ShippingMethod model)
        {
            try
            {
                if (model != null)
                {
                    if (model.ID > 0)
                    {
                        var shippingMethod = _shippingMethodRepository.Get(model.ID);
                        if (shippingMethod != null)
                        {
                            shippingMethod.Name = model.Name;
                            shippingMethod.Description = model.Description;
                            shippingMethod.Price = model.Price;
                            shippingMethod.RegionInfo = model.RegionInfo;
                            shippingMethod.Status = model.Status;
                            _shippingMethodRepository.Update(shippingMethod);
                            return Result.Success(shippingMethod, Resource.General_Success);
                        }
                    }
                    else
                    {
                        _shippingMethodRepository.Add(model);
                        return Result.Success(model, Resource.General_Success);
                    }
                }
            }
            catch (Exception ex)
            {
                _application.Log.Error("OrderService > SaveShippingMethod - Failure for shippingMethod:{0}", ex, model.ID);
            }
            return Result.Error(Resource.General_Error);
        }

        private decimal GetShippingCost(int shippingID, Basket basket)
        {
            if (shippingID > 0 && basket != null)
            {
                var shippingMethod = _shippingMethodRepository.Get(shippingID);
                if (shippingMethod != null && shippingMethod.ID > 0)
                {
                    var cost = shippingMethod.Price;
                    var discount = basket.Discounts.FirstOrDefault(i => i.DiscountType == DiscountType.FixedPriceShipping || i.DiscountType == DiscountType.Shipping);
                    if (discount != null)
                    {
                        var discountedCost = discount.DiscountType == DiscountType.FixedPriceShipping
                            ? discount.Factor : cost * (100 - discount.Factor) / 100;
                        cost = Math.Min(discountedCost, cost);
                    }
                    return cost;
                }
            }
            return 0;
        }

        private List<int> GetAddressRegions(IEnumerable<Address> addressList)
        {
            var result = new List<int>();
            if (addressList != null && addressList.Any())
            {
                foreach (var address in addressList)
                {
                    result.Add(address.CityID);
                    result.Add(address.CountyID);
                    result.Add(address.DistrictID);
                }
            }
            return result;
        }
    }
}