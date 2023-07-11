using System;
using Worchart.BL.Enum;
using Worchart.BL.Model;

namespace Worchart.BL.Project
{
    public class ProjectModel : IEntity
    {
        public virtual int ID { get; set; }

        public virtual string Reference { get; set; }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual DateTime DateCreated { get; set; }

        public virtual DateTime DateModified { get; set; }

        public virtual int CreatedBy { get; set; }

        public virtual int FirmID { get; set; }

        public virtual StatusType Status { get; set; }

        public virtual bool IsValid()
        {
            return Name.Valid();
        }
    }
}
