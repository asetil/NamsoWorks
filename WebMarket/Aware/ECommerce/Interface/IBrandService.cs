using System.Collections.Generic;
using Aware.ECommerce.Model;
using Aware.Util;

namespace Aware.ECommerce.Interface
{
    public interface IBrandService:IBaseService<Brand>
    {
        List<Brand> GetBrands(int page, int pageSize = 25);
        void RefreshProductBrand(string newName, string oldName);
    }
}