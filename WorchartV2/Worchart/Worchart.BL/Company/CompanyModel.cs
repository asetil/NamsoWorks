using System;
using Worchart.BL.Enum;
using Worchart.BL.Model;

namespace Worchart.BL.Company
{
    public class CompanyModel : IEntity
    {
        public virtual int ID { get; set; }

        public virtual string Name { get; set; }

        public virtual string CompanyCode { get; set; }

        public virtual int DomainID { get; set; }

        public virtual string LogoPath { get; set; }

        public virtual string PhoneNumber { get; set; }

        public virtual string WebAddress { get; set; }

        public virtual string Address { get; set; }

        public virtual DateTime DateCreated { get; set; }

        public virtual StatusType Status { get; set; }

       
        public virtual bool IsValid()
        {
            return Name.Valid() && CompanyCode.Valid();
        }
    }
}
