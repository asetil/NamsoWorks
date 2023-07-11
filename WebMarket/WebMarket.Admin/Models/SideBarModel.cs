using System.Collections.Generic;
using Aware.Authenticate.Model;
using Aware.ECommerce.Model;
using Aware.Util.Lookup;

namespace WebMarket.Admin.Models
{
    public class SideBarModel
    {
        public SideBarModel()
        {
            User = new CustomPrincipal();
        }

        public CustomPrincipal User { get; set; }
        public IEnumerable<Store> Stores { get; set; }
        public List<Lookup> RoleList { get; set; }
    }
}