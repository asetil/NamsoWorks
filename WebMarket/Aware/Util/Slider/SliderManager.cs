using System;
using System.Collections.Generic;
using Aware.Data;
using Aware.ECommerce;
using Aware.ECommerce.Util;
using Aware.Util.Enums;
using Aware.Util.Log;
using Aware.Util.Lookup;

namespace Aware.Util.Slider
{
    public class SliderManager :BaseService<SliderItem>, ISliderManager
    {
        private readonly IApplication _application;

        public SliderManager(IApplication application, IRepository<SliderItem> repository, ILogger logger)
            :base(repository,logger)
        {
            _application = application;
        }

        public List<SliderItem> GetCachedSliderItems(SliderType sliderType)
        {
            try
            {
                var key = string.Format(Constants.Slider_Cache, (int)sliderType);
                var result = _application.Cacher.Get<List<SliderItem>>(key);
                if (result == null)
                {
                    result = GetSliderItems(sliderType, Statuses.Active) ?? new List<SliderItem>();
                    _application.Cacher.Add(key, result);
                }
                return result;
            }
            catch (Exception ex)
            {
                _application.Log.Error("SliderManager > GetCachedSliderItems - Failed for sliderType:{0}", ex, sliderType);
            }
            return new List<SliderItem>();
        }

        public SliderManagementModel GetSliderManagementModel(SliderType type)
        {
            try
            {
                return new SliderManagementModel
                {
                    SliderType = type,
                    ItemList = GetSliderItems(type),
                    StatusList = _application.Lookup.GetLookups(LookupType.Status)
                };
            }
            catch (Exception ex)
            {
                _application.Log.Error("SliderManager > GetSliderManagementModel - Failed for sliderType:{0}", ex, type);
            }
            return null;
        }

        public List<SliderItem> GetSliderItems(SliderType sliderType, Statuses status = Statuses.None)
        {
            try
            {
                if (status != Statuses.None)
                {
                    return Repository.Where(i => i.Type == sliderType && i.Status == status).SortBy(f => f.SortOrder).ToList();
                }
                return Repository.Where(i => i.Type == sliderType).SortBy(f => f.SortOrder).ToList();
            }
            catch (Exception ex)
            {
                _application.Log.Error("SliderManager > GetSliderItems - Fail for sliderType:{0}", ex, sliderType);
            }
            return null;
        }

        public SliderItem GetSliderItem(int itemID, SliderType type)
        {
            try
            {
                if (itemID > 0)
                {
                    return Repository.First(i => i.ID == itemID && i.Type == type);
                }
            }
            catch (Exception ex)
            {
                _application.Log.Error("SliderManager > GetSliderItem - Fail for sliderID:{0}", ex, itemID);
            }
            return null;
        }

        protected override void OnBeforeUpdate(ref SliderItem existing, SliderItem model)
        {
            if (existing != null && model != null)
            {
                existing.Title = model.Title;
                existing.SubTitle = model.SubTitle;
                existing.ImagePath = model.ImagePath;
                existing.Url = model.Url;
                existing.SortOrder = model.SortOrder;
                existing.Status = model.Status;
            }
        }
    }
}