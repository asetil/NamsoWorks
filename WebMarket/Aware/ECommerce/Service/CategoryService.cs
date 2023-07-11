using System.Collections.Generic;
using System.Linq;
using Aware.ECommerce.Model;
using System;
using Aware.Data;
using Aware.Util;
using Aware.ECommerce.Interface;
using Aware.ECommerce.Util;
using Aware.Util.Model;
using Aware.ECommerce.Enums;
using Aware.ECommerce.Model.Custom;
using Aware.File;
using Aware.Util.Enums;
using Aware.Language;
using Aware.Util.Lookup;

namespace Aware.ECommerce.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepository<Category> _categoryRepository;
        private readonly ILanguageService _languageService;
        private readonly IApplication _application;
        private readonly IFileService _fileService;

        public CategoryService(ILanguageService languageService, IRepository<Category> categoryRepository, IApplication application, IFileService fileService)
        {
            _languageService = languageService;
            _categoryRepository = categoryRepository;
            _application = application;
            _fileService = fileService;
        }

        /// <summary>
        /// Get a cached category with specified id
        /// </summary>
        /// <param name="id">ID info for category</param>
        /// <param name="hierarchy">An enum value which specifiy getting sub categories strategy</param>
        /// <returns>Category object with ordered sub categories</returns>
        public Category GetCategory(int id, CategoryHierarchy hierarchy = CategoryHierarchy.None)
        {
            Category result = null;
            try
            {
                if (id > 0)
                {
                    var categories = GetCategories();
                    var category = categories.FirstOrDefault(i => i.ID == id);
                    if (category != null) { result = category.Clone(); }

                    if (result != null && hierarchy != CategoryHierarchy.None)
                    {
                        var subCategories = new List<Category>();
                        if (hierarchy == CategoryHierarchy.OnlyChildren)
                        {
                            subCategories = categories.Where(i => i.ParentID == category.ID).OrderBy(i => i.SortOrder).ToList();
                        }
                        else if (hierarchy == CategoryHierarchy.AllDescendants)
                        {
                            subCategories = categories.Where(i => i.SortOrder.StartsWith(category.SortOrder) && i.ID != category.ID).OrderBy(i => i.SortOrder).ToList();
                        }
                        result.SetSubCategories(subCategories);
                    }
                }
            }
            catch (Exception ex)
            {
                _application.Log.Error("CategoryService > GetCategory - Fail with id:{0}", ex, id);
                result = null;
            }
            return result;
        }

        public CategoryViewModel GetCategoryViewModel(int categoryID, ViewTypes viewType)
        {
            try
            {
                var parentHierarchy = GetCategoryHierarchy(categoryID);
                var category = parentHierarchy.LastOrDefault();

                var result = new CategoryViewModel()
                {
                    Category = category,
                    FileGallery = _fileService.GetGalleryForModel(category.ID, (int)RelationTypes.Category, category.ImageInfo, viewType, 3),
                    HierarchyInfo = string.Join(" / ", parentHierarchy.Select(i => i.Name)).Trim('/'),
                    HasMultiLanguage = _application.Site.UseMultiLanguage,
                    StatusList = _application.Lookup.GetLookups(LookupType.Status),
                    AllowEdit = viewType == ViewTypes.Editable
                };
                return result;
            }
            catch (Exception ex)
            {
                _application.Log.Error("CategoryService > GetCategoryViewModel - Fail with id:{0}", ex, categoryID);
            }
            return null;
        }

        /// <summary>
        /// Get all cached categories
        /// </summary>
        /// <param name="level">If specified filters results with specified level</param>
        /// <returns>List of Category object</returns>
        public List<Category> GetCategories(int level = 0, Func<Category, bool> predicate = null)
        {
            try
            {
                var result = _application.Cacher.Get<List<Category>>(Constants.CK_CategoryList);
                if (result == null)
                {
                    result = _categoryRepository.Where(i => i.ID > 0).SortBy(i => i.SortOrder).ToList();
                    _application.Cacher.Add(Constants.CK_CategoryList, result);
                }

                if (predicate != null)
                {
                    result = result.Where(predicate).ToList();
                }

                if (level > 0)
                {
                    result = result.Where(i => i.Level == level).ToList();
                }

                SetLanguageValues(ref result);
                return result;
            }
            catch (Exception ex)
            {
                _application.Log.Error("CategoryService > GetCategories - Fail", ex);
            }
            return new List<Category>();
        }

        public List<int> GetRelatedCategoryIDs(List<int> parentIDs)
        {
            var result = new List<int>();
            if (parentIDs != null && parentIDs.Any())
            {
                foreach (var categoryID in parentIDs)
                {
                    var category = GetCategory(categoryID, CategoryHierarchy.AllDescendants);
                    if (category != null)
                    {
                        result.Add(category.ID);
                        if (category.SubCategories != null && category.SubCategories.Any())
                        {
                            result.AddRange(category.SubCategories.Select(cid => cid.ID));
                        }
                    }
                }
            }
            return result.Distinct().ToList();
        }

        public List<Category> GetCategoryHierarchy(int categoryID)
        {
            try
            {
                var categories = GetCategories();
                if (categories != null && categories.Any())
                {
                    var list = new List<Category>();
                    while (categoryID > 0)
                    {
                        var category = categories.FirstOrDefault(i => i.ID == categoryID);
                        if (category == null) { break; }
                        categoryID = category.ParentID;
                        list.Add(category);
                    }
                    return list.OrderBy(i => i.SortOrder).ToList();
                }
            }
            catch (Exception ex)
            {
                _application.Log.Error("CategoryService > GetCategoryHierarchy - Fail with categoryID:{0}", ex, categoryID);
            }
            return new List<Category>();
        }

        public List<Category> GetMainCategories()
        {
            var result = _application.Cacher.Get<List<Category>>(Constants.CK_MainCategories);
            if (result == null)
            {
                var categories = GetCategories(0, i => i.Level <= 2 && i.Status == Statuses.Active);
                result = categories.Where(i => i.Level == 1).Select(c =>
                {
                    var cloned = c.Clone();
                    cloned.SetSubCategories(categories.Where(i => i.Level == 2 && i.ParentID == cloned.ID).ToList());
                    return cloned;
                }).ToList();
                _application.Cacher.Add(Constants.CK_MainCategories, result);
            }
            return result;
        }

        public Category Save(Category model)
        {
            try
            {
                if (model == null) { return null; }
                if (model.ID > 0)
                {
                    var category = _categoryRepository.Where(i => i.ID == model.ID).First();
                    if (category != null)
                    {
                        Mapper.Map(ref category, model);
                        _categoryRepository.Update(category);
                        return category;
                    }
                    throw new Exception("Kategori bulunamadı!");
                }

                model.SortOrder = GetNewSortOrder(model);
                _categoryRepository.Add(model);
                return model;
            }
            catch (Exception ex)
            {
                var id = model != null ? model.ID : 0;
                _application.Log.Error("{0} ID'li kategori düzenlenirken hata oluştu", ex, id);
            }
            return null;
        }

        public Result Delete(int categoryID)
        {
            try
            {
                if (categoryID > 0)
                {
                    //TODO:### CleanCode error
                    //var products = _productRepository.Find(i => i.CategoryID == categoryID);
                    //if (products != null && products.Any())
                    //{
                    //    return Result.Error("Bu kategoriye bağlı ürünler olduğu için kategori silinemez!");
                    //}

                    var hierarchy = _categoryRepository.Where(i => i.ID == categoryID || i.ParentID == categoryID).ToList();
                    if (hierarchy != null && hierarchy.Any(i => i.ID == categoryID))
                    {
                        var category = hierarchy.FirstOrDefault(i => i.ID == categoryID);
                        if (category != null && hierarchy.Count(i => i.ParentID == categoryID) == 0)
                        {
                            if (_categoryRepository.Delete(categoryID))
                            {
                                return Result.Success(null, Resource.General_Success);
                            }
                        }
                        else
                        {
                            return Result.Error("Kategorinin alt kategorileri olduğu için kategori silinemez!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _application.Log.Error(string.Format("DeleteCategory - Fail with ID : {0}", categoryID), ex);
            }
            return Result.Error(Resource.General_Error);
        }

        public bool RefreshHierarchy(int categoryID, int direction)
        {
            try
            {
                if (categoryID > 0)
                {
                    var category = GetCategory(categoryID, CategoryHierarchy.AllDescendants);
                    if (category != null && category.ID > 0)
                    {
                        Category nextCategory;
                        if (direction == 1)
                        {
                            nextCategory = _categoryRepository.Where(i => i.ParentID == category.ParentID && i.SortOrder.CompareTo(category.SortOrder) > 0)
                                                .SortBy(f => f.SortOrder).First();
                        }
                        else
                        {
                            nextCategory = _categoryRepository.Where(i => i.ParentID == category.ParentID && i.SortOrder.CompareTo(category.SortOrder) < 0)
                                                .SortBy(f => f.SortOrder, true).First();
                        }

                        if (nextCategory != null && nextCategory.ID > 0)
                        {
                            var subCategories = category.SubCategories;
                            foreach (var item in subCategories)
                            {
                                item.SortOrder = nextCategory.SortOrder + item.SortOrder.Substring(category.SortOrder.Length);
                                _categoryRepository.Update(item, false);
                            }

                            var nextSubCategories = GetCategory(nextCategory.ID, CategoryHierarchy.OnlyChildren).SubCategories;
                            foreach (var item in nextSubCategories)
                            {
                                item.SortOrder = category.SortOrder + item.SortOrder.Substring(nextCategory.SortOrder.Length);
                                _categoryRepository.Update(item, false);
                            }

                            var nSortOrder = nextCategory.SortOrder;
                            nextCategory.SortOrder = category.SortOrder;
                            _categoryRepository.Update(nextCategory, false);

                            category.SortOrder = nSortOrder;
                            _categoryRepository.Update(category);
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _application.Log.Error("CategoryService > RefreshHierarchy - Fail with ID : {0}", ex, categoryID);
            }
            return false;
        }

        private string GetNewSortOrder(Category category)
        {
            var sortOrder = string.Empty;
            if (category != null)
            {
                Category lastCategory = null;
                if (category.ParentID == 0)
                {
                    lastCategory = GetCategories(1).LastOrDefault();
                }
                else
                {
                    var parentCategory = GetCategory(category.ParentID, CategoryHierarchy.OnlyChildren);
                    if (parentCategory == null) { throw new Exception("Parent category nor found!"); }

                    sortOrder = parentCategory.SortOrder;
                    if (parentCategory.SubCategories != null && parentCategory.SubCategories.Any())
                    {
                        lastCategory = parentCategory.SubCategories.LastOrDefault();
                    }
                }

                var subOrder = "001";
                if (lastCategory != null && !string.IsNullOrEmpty(lastCategory.SortOrder))
                {
                    var lastSortOrder = lastCategory.SortOrder;
                    subOrder = (lastSortOrder.Substring(lastSortOrder.Length - 3).Int() + 1).ToString().PadLeft(3, '0');
                }
                sortOrder = string.Format("{0}{1}", sortOrder, subOrder);
            }
            return sortOrder;
        }

        private void SetLanguageValues(ref List<Category> categoryList)
        {
            if (categoryList != null && categoryList.Any())
            {
                var languageValues = _application.Cacher.Get<List<Language.Model.LanguageValue>>(Constants.CK_CategoryLanguageValues);
                if (languageValues == null)
                {
                    var idList = categoryList.Select(i => i.ID).ToList();
                    languageValues = _languageService.GetLanguageValues((int)RelationTypes.Category, idList, Constants.DefaultLangID);
                    _application.Cacher.Add(Constants.CK_CategoryLanguageValues, languageValues);
                }

                if (languageValues != null && languageValues.Any())
                {
                    categoryList = categoryList.Select(i =>
                    {
                        i.Name = _languageService.GetFieldContent(languageValues, i.ID, "Name", i.Name);
                        return i;
                    }).ToList();
                }
            }
        }
    }
}