using System;
using System.Collections.Generic;
using System.Linq;
using Aware.Cache;
using Aware.Data;
using Aware.ECommerce.Interface;
using Aware.ECommerce.Model;
using Aware.ECommerce.Util;
using Aware.Util;
using Aware.Util.Log;
using Aware.Util.Model;
using Aware.Util.Enums;

namespace Aware.ECommerce.Service
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IRepository<Favorite> _favoriteRepository;
        private readonly ICacher _cacher;
        private readonly ILogger _logger;

        public FavoriteService( ICacher cacher, IRepository<Favorite> favoriteRepository,  ILogger logger)
        {
            _cacher = cacher;
            _favoriteRepository = favoriteRepository;
            _logger = logger;
        }

        public List<int> GetUserFavorites(int userID)
        {
            var result = new List<int>();
            if (userID > 0)
            {
                var cacheKey = string.Format(Constants.CK_FavoriteProducts, userID);
                result = _cacher.Get<List<int>>(cacheKey);
                if (result == null)
                {
                    var favorites = _favoriteRepository.Where(i => i.UserID == userID && i.Status == Statuses.Active).ToList();
                    result = favorites.Select(f => f.ProductID).ToList();
                    _cacher.Add(cacheKey, result);
                }
            }
            return result;
        }
        public Result AddToFavorite(int userID, int productID, int storeID = 0)
        {
            try
            {
                if (userID > 0 && productID > 0)
                {
                    var favorite = _favoriteRepository.Where(i => i.UserID == userID && i.ProductID == productID).First();
                    if (favorite != null && favorite.Status == Statuses.Deleted)
                    {
                        favorite.Status = Statuses.Active;
                        _favoriteRepository.Update(favorite);
                    }
                    else if (favorite == null)
                    {
                        favorite = new Favorite()
                        {
                            UserID = userID,
                            ProductID = productID,
                            StoreID = storeID,
                            Status = Statuses.Active
                        };
                        _favoriteRepository.Add(favorite);
                    }

                    UpdateFavoriteCache(userID, productID, false);
                    return Result.Success(null, Resource.Basket_AddToFavoriteSuccessfull);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("FavoriteService > AddToFavorite - Fail for userID:{0}, productID:{1}", ex, userID, productID);
            }
            return Result.Error(Resource.Favorite_AddToFavoritesFailed);
        }
        public Result RemoveFavorites(int userID, string productIDs)
        {
            try
            {
                if (userID > 0 && !string.IsNullOrEmpty(productIDs.Trim(',')))
                {
                    var idList = productIDs.Trim(',').Split(',').Select(a => a.Int());
                    var favorites = _favoriteRepository.Where(i => i.UserID == userID && idList.Contains(i.ProductID)).ToList();
                    if (favorites != null && favorites.Any())
                    {
                        foreach (var favorite in favorites)
                        {
                            favorite.Status = Statuses.Deleted;
                            _favoriteRepository.Update(favorite, false);
                            UpdateFavoriteCache(userID, favorite.ProductID, true);
                        }
                        _favoriteRepository.Save();
                    }
                    return Result.Success(idList, Resource.Basket_RemoveFromFavoriteSuccessfull);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("FavoriteService > RemoveFavorites - Fail for userID:{0}, productIDs:{1}", ex, userID, productIDs);
            }
            return Result.Error(Resource.Basket_RemoveFromFavoriteFailed);
        }

        private bool UpdateFavoriteCache(int userID, int productID, bool removed)
        {
            if (userID > 0)
            {
                var cacheKey = string.Format(Constants.CK_FavoriteProducts, userID);
                if (productID == 0 && removed)
                {
                    _cacher.Remove(cacheKey);
                }

                var result = _cacher.Get<List<int>>(cacheKey) ?? new List<int>();
                if (removed)
                {
                    result.Remove(productID);
                }
                else if (result.All(i => i != productID))
                {
                    result.Add(productID);
                }
                _cacher.Add(cacheKey, result);
                return true;
            }
            return false;
        }
    }
}