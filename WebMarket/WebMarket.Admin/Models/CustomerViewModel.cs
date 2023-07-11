using System.Collections.Generic;
using Aware.Crm.Model;
using Aware.Util.Lookup;

namespace WebMarket.Admin.Models
{
    public class CustomerViewModel
    {
        public Customer Customer { get; set; }
        public List<Lookup> StatusList { get; set; }
        public bool IsSuper { get; set; }
    }
}