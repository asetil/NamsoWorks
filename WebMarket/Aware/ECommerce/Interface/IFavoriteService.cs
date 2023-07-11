using System.Collections.Generic;
using Aware.Util.Model;

namespace Aware.ECommerce.Interface
{
    public interface IFavoriteService
    {
        List<int> GetUserFavorites(int userID);
        Result AddToFavorite(int userID, int productID, int storeID = 0);
        Result RemoveFavorites(int userID, string productIDs);
    }
}