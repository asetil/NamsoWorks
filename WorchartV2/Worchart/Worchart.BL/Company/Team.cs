using System;
using Worchart.BL.Enum;
using Worchart.BL.Model;

namespace Worchart.BL.Company
{
    public class Team : IEntity
    {
        public virtual int ID { get; set; }

        public virtual int CompanyID { get; set; }

        public virtual string Name { get; set; }

        public virtual string ShortName { get; set; }

        public virtual string Description { get; set; }

        public virtual string Logo { get; set; }

        public virtual DateTime CreateDate { get; set; }

        public virtual int CreatedBy { get; set; }

        public virtual StatusType Status { get; set; }

        public virtual bool IsValid()
        {
            return CompanyID > 0 && Name.Valid() && ShortName.Valid() && CreatedBy > 0;
        }
    }
}
