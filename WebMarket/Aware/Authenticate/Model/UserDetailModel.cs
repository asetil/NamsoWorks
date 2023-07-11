using System.Collections.Generic;
using Aware.Crm.Model;
using Aware.Util.Lookup;

namespace Aware.Authenticate.Model
{
    public class UserDetailModel
    {
        public User User { get; set; }
        public List<Lookup> UserRoleList { get; set; }
        public List<Lookup> TitleList { get; set; }
        public List<Lookup> StatusList { get; set; }
        public bool AllowPasswordChange { get; set; }
        public bool IsSuper { get; set; }
        public Customer Customer { get; set; }
    }
}