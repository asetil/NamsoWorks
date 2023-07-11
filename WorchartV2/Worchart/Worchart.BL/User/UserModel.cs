using System;
using Worchart.BL.Constants;
using Worchart.BL.Enum;
using Worchart.BL.Exceptions;
using Worchart.BL.Model;

namespace Worchart.BL.User
{
    public class UserModel : IEntity
    {
        public virtual int ID { get; set; }

        public virtual int CompanyID { get; set; }

        public virtual string Name { get; set; }

        public virtual string Email { get; set; }

        public virtual string Password { get; set; }

        public virtual int Gender { get; set; }

        public virtual DateTime DateCreated { get; set; }

        public virtual DateTime DateModified { get; set; }

        public virtual DateTime LastVisit { get; set; }

        public virtual CustomerRole Role { get; set; }

        public virtual StatusType Status { get; set; }

        public virtual bool IsValid()
        {
            if (!Email.ValidEmail())
            {
                throw new WorchartException(ErrorConstants.InvalidEmail);
            }
            return Password.Valid() && Name.Valid();
        }
    }
}
