using System.Collections.Generic;
using Aware.Util.Enums;

namespace Aware.ECommerce.Model
{
    public class Category
    {
        public virtual int ID { get; set; }
        public virtual string Name { get; set; }
        public virtual int ParentID { get; set; }
        public virtual string SortOrder { get; set; }
        public virtual string ImageInfo { get; set; } //JSON
        public virtual Statuses Status { get; set; }

        public virtual int Level
        {
            get
            {
                if (!string.IsNullOrEmpty(SortOrder))
                {
                    return SortOrder.Length / 3;
                }
                return 9999;
            }
        }

        public virtual List<Category> SubCategories
        {
            get
            {
                return _subCategories;
            }
        }

        public virtual Category Clone()
        {
            var result = MemberwiseClone() as Category;
            return result;
        }
       
        private List<Category> _subCategories;
        public virtual Category SetSubCategories(List<Category> subCategories)
        {
            _subCategories = subCategories;
            return this;
        }
    }
}
