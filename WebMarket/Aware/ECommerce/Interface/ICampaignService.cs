using System.Collections.Generic;
using System.Web;
using Aware.ECommerce.Model;
using Aware.ECommerce.Model.Custom;
using Aware.Util.Model;

namespace Aware.ECommerce.Interface
{
    public interface ICampaignService
    {
        CampaignListModel GetManagerCampaigns(int managerID);
        Campaign GetAdminCampaign(int campaignID,int ownerID);
        List<Campaign> GetActiveCampaigns();
        Campaign GetActiveCampaign(int campaignID);
        List<Discount> GetBasketDiscounts(int basketID);
        List<Discount> CalculateBasketDiscounts(Basket basket);
        Result Save(Campaign model, int ownerID, HttpRequestBase request);
        Result DeleteCampaign(int campaignID);
        bool SaveDiscounts(Basket basket, Order order);
        Result UseCoupon(int userID, int basketID, string code);
        IEnumerable<Discount> GetUsedCoupons(int basketID);
    }
}