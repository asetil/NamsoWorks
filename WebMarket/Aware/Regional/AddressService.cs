using System;
using System.Collections.Generic;
using System.Linq;
using Aware.Cache;
using Aware.Data;
using Aware.ECommerce.Model;
using Aware.ECommerce.Util;
using Aware.Regional.Model;
using Aware.Util;
using Aware.Util.Log;
using Aware.Util.Model;
using Aware.Util.Enums;

namespace Aware.Regional
{
    public class AddressService : IAddressService
    {
        private readonly IRepository<Address> _addressRepository;
        private readonly IRepository<Region> _regionRepository;
        private readonly ICacher _cacher;
        private readonly ILogger _logger;

        public AddressService(ICacher cacher, IRepository<Address> addressRepository, IRepository<Region> regionRepository, ILogger logger)
        {
            _cacher = cacher;
            _addressRepository = addressRepository;
            _regionRepository = regionRepository;
            _logger = logger;
        }

        #region Region

        public List<Region> SearchRegions(string keyword, int? parentID, RegionType regionType = RegionType.None, int size=0)
        {
            var query = _regionRepository.Where(i => i.ID > 0);
            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                query.FilterBy(i => i.Name.ToLower().Contains(keyword));
            }

            if (parentID.HasValue)
            {
                query.FilterBy(i => i.ParentID == parentID.Value);
            }

            if (regionType != RegionType.None)
            {
                query.FilterBy(i => i.Type == regionType);
            }

            if (size > 0)
            {
                query.SetPaging(1, size);
            }
            return query.ToList();
        }

        public List<Region> GetRegionsWithIDs(List<int> regionIDs)
        {
            try
            {
                var result = _cacher.Get<List<Region>>(Constants.CK_Regions);
                if (result == null || !result.Any())
                {
                    result = _regionRepository.Where(i => i.Status == Statuses.Active).ToList();
                    if (result != null && result.Any())
                    {
                        _cacher.Add(Constants.CK_Regions, result);
                    }
                }

                if (regionIDs != null && regionIDs.Any())
                {
                    return result.Where(i => regionIDs.Contains(i.ID)).ToList();
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error("AddressManager > GetRegionList2 - failed for {0}", ex, string.Join(",", regionIDs));
            }
            return null;
        }

        public Region GetRegion(int regionID)
        {
            var region = GetRegionsWithIDs(new List<int> { regionID }).FirstOrDefault(i => i.ID == regionID);
            return region ?? new Region();
        }

        public Result SaveRegion(Region model)
        {
            try
            {
                if (model != null && !string.IsNullOrEmpty(model.Name))
                {
                    if (model.ID > 0)
                    {
                        var region = _regionRepository.Get(model.ID);
                        if (region != null)
                        {
                            region.Name = model.Name;
                            _regionRepository.Update(region);
                            model = region;
                        }
                    }
                    else
                    {
                        model.Status = Statuses.Active;
                        model.SortOrder = "001";
                        _regionRepository.Add(model);
                    }
                    return Result.Success(model, Resource.General_Success);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("AddresManager > SaveRegion - failed", ex);
            }
            return Result.Error(Resource.General_Error);
        }

        public Result DeleteRegion(int id)
        {
            try
            {
                if (id > 0)
                {
                    var region = _regionRepository.Get(id);
                    if (region != null)
                    {
                        var subRegions = _regionRepository.Where(i => i.ParentID == region.ID).ToList();
                        if (subRegions != null && subRegions.Any())
                        {
                            return Result.Error(Resource.RegionDelete_RegionHasSubegions);
                        }

                        var success = _regionRepository.Delete(id);
                        return success ? Result.Success() : Result.Error(Resource.General_Error);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("AddresManager > DeleteRegion - failed", ex);
            }
            return Result.Error(Resource.General_Error);
        }

        #endregion

        #region Address
        public void LoadAddressRecipe(ref Address address)
        {
            if (address != null && address.DistrictID>0)
            {
                var district = GetRegion(address.DistrictID);
                var county = GetRegion(district.ParentID);
                var city = GetRegion(county.ParentID);
                var cityDescription = string.Format("{0} - {1} / {2}", district.Name, county.Name, city.Name);

                address.CountyID = county.ID;
                address.CityID = city.ID;
                address.DisplayText = string.Format("<b>{0}</b><br><span title='{1}'>{2}</span><br>{3}<br>Tel: {4}", address.Name, address.Description, address.Description.Short(75), cityDescription, address.Phone);
            }
        }

        public AddressViewModel GetUserAddress(int addressID, int userID)
        {
            try
            {
                var address = new Address();
                if (addressID > 0 && userID > 0)
                {
                    address = _addressRepository.Where(i => i.ID == addressID && i.UserID == userID).First();
                    if (address == null || address.ID == 0) { throw new Exception("Address not found!" + addressID); }
                    LoadAddressRecipe(ref address);
                }

                var result = new AddressViewModel()
                {
                    Address = address,
                    DistrictInfo = address.DistrictID == 0 ? new Region() : GetRegion(address.DistrictID)
                };
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error("AddressService > GetUserAddress - Fail for ID:{0}, userID:{1}", ex, addressID, userID);
            }
            return null;
        }

        public OrderViewModel GetUserAddresses(int userID, bool loadRecipe)
        {
            try
            {
                if (userID > 0)
                {
                    var addressList = _addressRepository.Where(i => i.UserID == userID && i.Status == Statuses.Active).ToList();
                    if (loadRecipe)
                    {
                        addressList = addressList.Select(i =>
                        {
                            LoadAddressRecipe(ref i);
                            return i;
                        }).ToList();
                    }

                    return new OrderViewModel()
                    {
                        AddressList = addressList,
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.Error("AddressService > GetUserActiveAddresses - failed for userID:{0}", ex, userID);
            }
            return null;
        }

        public List<Address> GetOrderAddresses(Order order)
        {
            List<Address> result = null;
            if (order != null)
            {
                result = _addressRepository.Where(i => i.ID == order.ShippingAddressID || i.ID == order.BillingAddressID).ToList().Select(i =>
                    {
                        LoadAddressRecipe(ref i);
                        return i;
                    }).ToList();
            }
            return result;
        }

        public Result SaveAddress(Address model, int userID)
        {
            try
            {
                if (model != null && userID > 0)
                {
                    model.UserID = userID;
                    if (model.ID > 0)
                    {
                        var address = _addressRepository.Get(model.ID);
                        if (address != null)
                        {
                            address.Name = model.Name;
                            address.Phone = model.Phone;
                            address.Description = model.Description;
                            address.DistrictID = model.DistrictID;
                            _addressRepository.Update(address);
                        }
                    }
                    else
                    {
                        var userAddresses = _addressRepository.Where(i => i.UserID == userID && i.Status == Statuses.Active).ToList();
                        if (userAddresses != null && userAddresses.Count() >= 5)
                        {
                            return Result.Error(Resource.Address_MaxAddressCountExceeded);
                        }

                        model.Status = Statuses.Active;
                        _addressRepository.Add(model);
                    }
                    return Result.Success(model, Resource.General_Success);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("AddressService > EditAddress - Fail for addressID:{0} and userID:{1}", ex, model.ID, userID);
            }
            return Result.Error(Resource.Address_AddressCannotBeEdited);
        }

        public Result DeleteAddress(int addressID, int userID)
        {
            try
            {
                if (addressID > 0 && userID > 0)
                {
                    var address = _addressRepository.Where(i => i.ID == addressID && i.UserID == userID).First();
                    if (address != null)
                    {
                        address.Status = Statuses.Deleted;
                        _addressRepository.Update(address);
                        return Result.Success(address, Resource.General_Success);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("AddressService > DeleteAddress - Fail for ID:{0}, userID:{1}!", ex, addressID, userID);
            }
            return Result.Error(Resource.Address_AddressCannotBeDeleted);
        }

        #endregion
    }
}