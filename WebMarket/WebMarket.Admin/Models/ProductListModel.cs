using System.Collections.Generic;
using Aware.ECommerce.Model;
using Aware.Util;
using Aware.Util.Enums;

namespace WebMarket.Admin.Models
{
    public class ProductListModel
    {
        public IEnumerable<Product> ProductList { get; set; }

        public IEnumerable<Category> CategoryList { get; set; }

        public Statuses OwnerStatus { get; set; }
    }
}