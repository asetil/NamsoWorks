using Aware.Util.Enums;

namespace Aware.Payment.Model
{
    public class BankInfo
    {
        public virtual int ID { get; set; }
        public virtual string Name { get; set; }
        public virtual string BranchName { get; set; }
        public virtual string IBAN { get; set; }
        public virtual string AccountNumber { get; set; }
        public virtual string Description { get; set; }
        public virtual string ImageUrl { get; set; }
        public virtual Statuses Status { get; set; }
    }
}