using System.Collections.Generic;
using Aware.Util.Lookup;

namespace Aware.Authenticate.Model
{
    public class ManagerListModel
    {
        public List<User> ManagerList { get; set; }
        public bool IsSuper { get; set; }
        public int CustomerID { get; set; }
        public List<Lookup> TitleList { get; set; }

    }
}