using Aware.Util.Enums;

namespace Aware.ECommerce.Model
{
    public class Brand:IEntity
    {
        public virtual int ID { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual string ImagePath { get; set; }
        public virtual Statuses Status { get; set; }
    }
}
