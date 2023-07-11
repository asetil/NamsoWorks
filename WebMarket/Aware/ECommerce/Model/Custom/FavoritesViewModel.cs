using System.Collections.Generic;

namespace Aware.ECommerce.Model
{
    public class FavoritesViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Store> Stores { get; set; }
    }
}
