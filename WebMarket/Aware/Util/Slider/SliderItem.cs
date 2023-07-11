using Aware.ECommerce.Model;
using Aware.Util.Enums;

namespace Aware.Util.Slider
{
    public class SliderItem : IEntity
    {
        public virtual int ID { get; set; }
        public virtual SliderType Type { get; set; }
        public virtual string Title { get; set; }
        public virtual string SubTitle { get; set; }
        public virtual string ImagePath { get; set; }
        public virtual string Url { get; set; }
        public virtual string SortOrder { get; set; }
        public virtual Statuses Status { get; set; }
    }
}
