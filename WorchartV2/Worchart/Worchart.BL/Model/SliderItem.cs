using System;
using Worchart.BL.Enum;

namespace Worchart.BL.Model
{
    public class SliderItem : IEntity
    {
        public virtual int ID { get; set; }

        public virtual string Title { get; set; }

        public virtual string Subtitle { get; set; }

        public virtual string Description { get; set; }

        public virtual string ImagePath { get; set; }

        public virtual string ActionUrl { get; set; }

        public virtual int SortOrder { get; set; }

        public virtual StatusType Status { get; set; }

        public virtual bool IsValid()
        {
            return Title.Valid();
        }
    }
}
