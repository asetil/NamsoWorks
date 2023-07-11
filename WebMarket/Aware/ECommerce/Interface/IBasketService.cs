using Aware.ECommerce.Model;
using Aware.Util.Enums;
using Aware.Util.Model;

namespace Aware.ECommerce.Interface
{
    public interface IBasketService
    {
        Basket GetUserBasket(int userID, int basketID = 0, bool createEnabled = false);
        Basket GetBasketWithDiscounts(int userID, int basketID);
        Basket GetBasketSummary(int userID, int basketID);
        Result CheckBasketBeforePurchase(int userID, Basket basket = null);
        Result AddItemToBasket(int userID,int itemID, decimal quantity, string variantSelection);
        Result ChangeBasketItemQuantity(int userID, int basketID, int basketItemID, decimal quantity);
        Result DeleteBasketItem(int userID, int basketID, int basketItemID);
        Basket CreateBasket(int userID, string name = "", Statuses status = Statuses.Active);
        Result ClearBasket(int userID, int basketID, bool deleteBasket);
        bool SetBasketOrdered(Order order);
        Result AddFavoritesToBasket(int userID, int storeID, string productIDs);
        Result UseCoupon(int userID, int basketID, string code);
    }
}