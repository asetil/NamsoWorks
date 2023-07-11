using System;
using System.Linq;
using Aware.Search;
using Aware.Util.Enums;
using Aware.Dependency;
using Aware.ECommerce.Interface;
using System.Collections.Generic;

namespace CleanFramework.Business.Model
{
    public class EntrySearchParams : SearchParams<Entry>
    {
        public int CategoryID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public String Tag { get; set; }

        public EntrySearchParams()
        {
        }

        public EntrySearchParams(int categoryID = 0, string keyword = "", string tag = "")
        {
            CategoryID = categoryID;
            Keyword = keyword;
            Tag = tag;
        }

        public override SearchHelper<Entry> PrepareFilters()
        {
            var searchHelper = SearchHelper;
            if (IDs != null && IDs.Any())
            {
                searchHelper.FilterBy(i => IDs.Contains(i.ID));
            }

            if (UserID > 0)
            {
                searchHelper.FilterBy(i => i.UserID == UserID);
            }

            if (!string.IsNullOrEmpty(Keyword))
            {
                var key = Keyword.Trim().ToLowerInvariant();
                searchHelper.FilterBy(i => i.Name.ToLower().Contains(key) || i.Summary.ToLower().Contains(key) || i.Keywords.ToLower().Contains(key));
            }

            if (!string.IsNullOrEmpty(Tag))
            {
                var key = Tag.Trim().ToLowerInvariant();
                searchHelper.FilterBy(i => i.Keywords.ToLower().Contains(key));
            }

            if (StartDate != DateTime.MinValue)
            {
                searchHelper.FilterBy(i => i.DateCreated >= StartDate);
            }

            if (EndDate != DateTime.MinValue)
            {
                searchHelper.FilterBy(i => i.DateCreated <= EndDate);
            }

            if (Status.HasValue && Status.Value != Statuses.None)
            {
                searchHelper.FilterBy(i => i.Status == Status.Value);
            }

            if (CategoryID > 0)
            {
                var categoryService = WindsorBootstrapper.Resolve<ICategoryService>();
                var categorIDs = categoryService.GetRelatedCategoryIDs(new List<int>() { CategoryID });
                searchHelper.FilterBy(i => categorIDs.Contains(i.CategoryID));
            }
            return searchHelper;
        }

        public string GetTitle(string defaultValue)
        {
            var result = string.Empty;
            if (CategoryID > 0)
            {
                var categoryService = WindsorBootstrapper.Resolve<ICategoryService>();
                var category = categoryService.GetCategory(CategoryID);
                result += category.Name;
            }

            if (!string.IsNullOrEmpty(Tag))
            {
                result += "|" + Tag.Trim();
            }

            if (!string.IsNullOrEmpty(Keyword))
            {
                result += "|" + Keyword.Trim();
            }

            if (!string.IsNullOrEmpty(result))
            {
                result = string.Format("{0} Makaleleri", result.Trim('|').Replace("|", ", "));
            }

            result = string.IsNullOrEmpty(result) ? defaultValue : result;
            return result;
        }
    }
}
