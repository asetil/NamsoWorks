using System;
using Worchart.BL.Enum;
using Worchart.BL.Model;

namespace Worchart.BL.Project
{
    public class ProjectRequirementModel : IEntity
    {
        public virtual int ID { get; set; }

        public virtual int ProjectID { get; set; }

        public virtual string Requirement { get; set; }

        public virtual string Notes { get; set; }

        public virtual DateTime DateCreated { get; set; }

        public virtual DateTime DateModified { get; set; }
        
        public virtual ProjectRequirementStatus Status { get; set; }

        public virtual bool IsValid()
        {
            return Requirement.Valid() && ProjectID > 0;
        }
    }
}
