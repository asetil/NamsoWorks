using System.Collections.Generic;
using System.Linq;
using Aware.Authenticate.Model;
using Aware.ECommerce.Model;
using Aware.Util.Model;

namespace WebMarket.Models
{
    public class UserViewModel
    {
        public User User { get; set; }
        public List<Aware.ECommerce.Model.SimpleItem> PermissionList { get; set; }
        public Result Result { get; set; }

        public bool HasPermissions
        {
            get { return PermissionList != null && PermissionList.Any(); }
        }
    }
}