using System.Collections.Generic;
using System.Linq;
using Aware.ECommerce.Model;
using Aware.Util.Lookup;

namespace WebMarket.Admin.Models
{
    public class ProductViewModel
    {
        public Product Product { get; set; }
        public List<Category> CategoryList { get; set; }
        public List<Brand> BrandList { get; set; }
        public List<Lookup> StatusList { get; set; }
        public List<Lookup> MeasureUnits { get; set; }
        public bool HasMultiLanguage { get; set; }

        public Brand SelectedBrand
        {
            get
            {
                Brand brand = null;
                if (Product != null && !string.IsNullOrEmpty(Product.Brand) && BrandList != null)
                {
                    brand = BrandList.FirstOrDefault(i => i.Name == Product.Brand);
                }
                return brand ?? new Brand();
            }
        }
    }
}
