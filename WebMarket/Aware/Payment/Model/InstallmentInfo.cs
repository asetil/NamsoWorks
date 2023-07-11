using Aware.Util;
using Aware.Util.Enums;

namespace Aware.Payment.Model
{
    public class InstallmentInfo
    {
        public virtual int ID { get; set; }
        public virtual int PosID { get; set; }
        public virtual string Name { get; set; }
        public virtual int Count { get; set; }
        public virtual decimal Commission { get; set; }
        public virtual Statuses Status { get; set; }
    }
}