using System.Collections.Generic;
using System.Linq;
using Aware.Dependency;
using Aware.ECommerce.Interface;
using Aware.ECommerce.Model;
using Aware.Search;
using Aware.Util.Enums;

namespace Aware.ECommerce.Search
{
    public class ProductSearchParams : SearchParams<Product>
    {
        public string Name { get; set; }
        public IEnumerable<int> CategoryIDs { get; set; }
        public IEnumerable<int> PropertyIDs { get; set; }
        public string Barcode { get; set; }
        public string Rating { get; set; } //[25:45] => 25 ile 45 TL arası olan
        public bool SearchCategories { get; set; }

        public override SearchHelper<Product> PrepareFilters()
        {
            var searchHelper = SearchHelper;
            if (!string.IsNullOrEmpty(Keyword))
            {
                searchHelper.FilterBy(i => i.Name.ToLower().Contains(Keyword.ToLower()));
            }

            if (!string.IsNullOrEmpty(Barcode))
            {
                searchHelper.FilterBy(i => i.Barcode.Contains(Barcode));
            }

            if (IDs != null && IDs.Any())
            {
                searchHelper.FilterBy(i => IDs.Contains(i.ID));
            }

            var hierarchicCategoryIDs = GetHierarchicalCategories();
            if (hierarchicCategoryIDs != null && hierarchicCategoryIDs.Any())
            {
                searchHelper.FilterBy(i => hierarchicCategoryIDs.Contains(i.CategoryID));
            }

            if (Status.HasValue && Status.Value != Statuses.None)
            {
                searchHelper.FilterBy(i => i.Status == Status.Value);
            }
            return searchHelper;
        }

        public ProductSearchParams(string keyword = "", int page = 1, int size = 25, string barcode = "")
        {
            Keyword = keyword;
            Barcode = barcode;
            SetPaging(page, size);
        }

        public ProductSearchParams WithCategory(int? categoryID, IEnumerable<int> categoryIDs = null)
        {
            var result = (CategoryIDs ?? new List<int>()).ToList();
            if (categoryIDs != null) { result.AddRange(categoryIDs); }
            if (categoryID.HasValue && categoryID.Value > 0) { result.Add(categoryID.Value); }

            CategoryIDs = result;
            return this;
        }


        private List<int> GetHierarchicalCategories()
        {
            if (CategoryIDs != null && CategoryIDs.Any())
            {
                var categoryService = WindsorBootstrapper.Resolve<ICategoryService>();
                return categoryService.GetRelatedCategoryIDs(CategoryIDs.ToList());
            }
            return null;
        }
    }
}
