using System;
using Worchart.BL.Enum;
using Worchart.BL.Model;
using Worchart.BL.User;

namespace Worchart.BL.Company
{
    public class TeamUser : IEntity
    {
        public virtual int ID { get; set; }

        public virtual int TeamID { get; set; }

        public virtual int UserID { get; set; }

        public virtual int RoleID { get; set; }

        public virtual DateTime CreateDate { get; set; }

        public virtual int CreatedBy { get; set; }

        public virtual StatusType Status { get; set; }

        public virtual UserModel User { get; set; }

        public virtual Item Role { get; set; }

        public virtual bool IsValid()
        {
            return TeamID > 0 && UserID > 0 && RoleID > 0;
        }
    }
}
