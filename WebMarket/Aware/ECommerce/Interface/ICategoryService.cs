using System;
using System.Collections.Generic;
using Aware.ECommerce.Model;
using Aware.Util.Model;
using Aware.ECommerce.Enums;
using Aware.ECommerce.Model.Custom;
using Aware.Util.Enums;

namespace Aware.ECommerce.Interface
{
    public interface ICategoryService
    {
        Category GetCategory(int id, CategoryHierarchy hierarchy = CategoryHierarchy.None);
        CategoryViewModel GetCategoryViewModel(int categoryID, ViewTypes viewType);
        List<Category> GetCategories(int level = 0,Func<Category,bool> predicate=null);
        List<int> GetRelatedCategoryIDs(List<int> parentIDs);
        List<Category> GetCategoryHierarchy(int categoryID);
        List<Category> GetMainCategories();
        Category Save(Category model);
        Result Delete(int categoryID);
        bool RefreshHierarchy(int categoryID, int direction);
    }
}