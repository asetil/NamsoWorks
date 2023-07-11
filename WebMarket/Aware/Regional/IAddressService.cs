using System.Collections.Generic;
using Aware.ECommerce.Model;
using Aware.Regional.Model;
using Aware.Util.Model;
using Aware.Util.Enums;

namespace Aware.Regional
{
    public interface IAddressService
    {
        void LoadAddressRecipe(ref Address address);
        AddressViewModel GetUserAddress(int addressID, int userID);
        OrderViewModel GetUserAddresses(int userID, bool loadRecipe);
        List<Address> GetOrderAddresses(Order order);
        Result SaveAddress(Address model, int userID);
        Result DeleteAddress(int addressID, int userID);

        List<Region> SearchRegions(string keyword, int? parentID, RegionType regionType = RegionType.None, int size=0);
        List<Region> GetRegionsWithIDs(List<int> regionIDs);
        Region GetRegion(int regionID);
       
        Result SaveRegion(Region model);
        Result DeleteRegion(int id);
    }
}