using System.Collections.Generic;
using Aware.Util.Enums;
using Aware.Util.Model;

namespace Aware.Util.Slider
{
    public interface ISliderManager : IBaseService<SliderItem>
    {
        List<SliderItem> GetCachedSliderItems(SliderType sliderType);
        SliderManagementModel GetSliderManagementModel(SliderType type);
        List<SliderItem> GetSliderItems(SliderType sliderType, Statuses status = Statuses.None);
        SliderItem GetSliderItem(int itemID, SliderType type);
    }
}
