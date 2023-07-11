using System;
using Worchart.BL.Enum;

namespace Worchart.BL.User
{
    public class AuthorizedCustomerModel
    {
        public int ID { get; set; }

        public string CustomerToken { get; set; }

        public int CompanyID { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public StatusType Status { get; set; }

        public DateTime ExpireTime { get; set; }

        public CustomerRole Role { get; internal set; }
    }
}
