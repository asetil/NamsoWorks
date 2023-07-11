using Aware.Util.Enums;

namespace Aware.ECommerce.Model
{
    public class Favorite
    {
        public virtual int ID { get; set; }
        public virtual int UserID { get; set; }
        public virtual int ProductID { get; set; }
        public virtual int StoreID { get; set; }
        public virtual Statuses Status { get; set; }
    }
}