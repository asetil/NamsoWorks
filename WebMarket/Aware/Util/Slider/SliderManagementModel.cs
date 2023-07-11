using System.Collections.Generic;
using Aware.Util.Enums;

namespace Aware.Util.Slider
{
    public class SliderManagementModel
    {
        public SliderType SliderType { get; set; }
        public List<SliderItem> ItemList { get; set; }
        public List<Lookup.Lookup> StatusList { get; set; }
    }
}