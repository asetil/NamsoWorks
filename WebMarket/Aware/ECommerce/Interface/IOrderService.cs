using System.Collections.Generic;
using Aware.ECommerce.Model;
using Aware.ECommerce.Search;
using Aware.Util.Model;
using Aware.ECommerce.Enums;

namespace Aware.ECommerce.Interface
{
    public interface IOrderService
    {
        OrderSearchResult SearchOrders(OrderSearchParams searchParams,int customerID);
        OrderViewModel GetOrderViewModel(int orderID, int userID, OrderStatuses status = 0, bool loadUserInfo = false);
        Result GetRawOrder(int userID);
        Order GetUserOrder(int userID, int orderID);
        List<Order> GetUserOrders(int userID);

        Result Approve(int userID, int orderID);
        Result SavePaymentInfo(int userID, int orderID, PaymentType paymentType, int subPaymentType, int installment, out bool savedBefore);
        Result ChangeOrderStatus(int orderID, OrderStatuses status);
        Result CancelOrder(int userID, int orderID);
        Result SaveOrder(Order model, int userID);
        ShippingMethod GetShippingMethod(int id);
        List<ShippingMethod> GetAllShippingMethods();
        List<ShippingMethod> GetRegionShippingMethods(int userID, List<int> regionIDs);
        Result SaveShippingMethod(ShippingMethod model);
    }
}