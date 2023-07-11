using Aware.Util;
using Aware.Util.Constants;
using Aware.Util.Enum;
using Aware.Util.Exceptions;
using System;

namespace Aware.Model
{
    public class User : BaseEntity
    {
        public int CompanyID { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Password { get; set; }

        public int Gender { get; set; }

        public DateTime LastVisit { get; set; }

        public CustomerRole Role { get; set; }

        public override bool IsValid()
        {
            if (!Email.ValidEmail())
            {
                throw new AwareException(ResultCodes.Error.InvalidEmail);
            }
            return Password.Valid() && Name.Valid();
        }
    }
}
