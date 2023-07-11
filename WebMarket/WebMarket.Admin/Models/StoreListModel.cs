using System.Collections.Generic;
using Aware.ECommerce.Model;

namespace WebMarket.Admin.Models
{
    public class StoreListModel
    {
        public IEnumerable<Store> StoreList { get; set; }
        public int CustomerID{ get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
        public int TotalSize { get; set; }
    }
}