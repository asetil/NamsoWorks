using System.Collections.Generic;
using System.Linq;
using Aware.Data;
using CleanFramework.Business.Model;
using Aware.Util.Model;
using System;
using Aware.Authenticate;
using Aware.Authenticate.Model;
using Aware.ECommerce;
using Aware.ECommerce.Enums;
using CleanFramework.Business.Util;
using Aware.Search;
using Aware.Util.Lookup;
using Aware.ECommerce.Interface;
using Aware.Util.Enums;

namespace CleanFramework.Business.Service
{
    public class EntryService : IEntryService
    {
        private readonly IRepository<Entry> _entryRepository;
        private readonly ICategoryService _categoryService;
        private readonly IUserService _userService;
        private readonly IApplication _application;

        public EntryService(IRepository<Entry> entryRepository, ICategoryService categoryService, IApplication application, IUserService userService)
        {
            _entryRepository = entryRepository;
            _categoryService = categoryService;
            _application = application;
            _userService = userService;
        }

        public SearchResult<Entry> Search(EntrySearchParams searchParams, bool loadCategories, bool loadAuthor)
        {
            searchParams = searchParams ?? new EntrySearchParams();
            var result = _entryRepository.Find(searchParams);
            if (result != null && result.HasResult)
            {
                if (loadCategories)
                {
                    var categoryList = _categoryService.GetCategories();
                    result.Results = result.Results.Select(entry =>
                    {
                        entry.Category = categoryList.FirstOrDefault(c => c.ID == entry.CategoryID);
                        return entry;
                    }).ToList();
                }

                if (loadAuthor)
                {
                    var authorList = GetAuthorList(result.Results.Select(i => i.UserID).ToList());
                    result.Results = result.Results.Select(entry =>
                    {
                        var author = authorList.FirstOrDefault(a => a.ID == entry.UserID);
                        entry.Author = author != null ? author.Name : string.Empty;
                        return entry;
                    }).ToList();
                }
            }
            return result;
        }

        public EntryListModel GetEntryList(EntrySearchParams searchParams)
        {
            searchParams = searchParams ?? new EntrySearchParams();
            searchParams.SortBy(i => i.DateCreated, true).WithCount();

            var searchResult = _entryRepository.Find(searchParams);
            return new EntryListModel()
            {
                SearchResult = searchResult,
                CategoryList = _categoryService.GetCategories(),
                StatusList = _application.Lookup.GetLookups(LookupType.Status)
            };
        }

        public EntryDetailModel GetEntryDetail(int id, int authorID)
        {
            var author = _userService.GetAdminUser(authorID);
            if (author != null)
            {
                var entry = id > 0 ? _entryRepository.Get(id) : new Entry { Status = Statuses.WaitingApproval };
                if (author.Role != UserRole.SuperUser && entry != null && entry.ID > 0 && entry.UserID != author.ID)
                {
                    entry = null;
                }

                return new EntryDetailModel()
                {
                    Entry = entry,
                    AuthorRole = author.Role,
                    CategoryList = _categoryService.GetCategories().Select(i => new Item(i.ID, i.Name)).ToList(),
                    StatusList = _application.Lookup.GetLookups(LookupType.Status)
                };
            }
            return null;
        }

        public EntryDisplayModel GetEntryDisplayModel(int id)
        {
            if (id > 0)
            {
                var entry = _entryRepository.First(i => i.ID == id && (i.Status == Statuses.Active || i.Status == Statuses.WaitingApproval));
                if (entry != null)
                {
                    var author = GetAuthorList(new List<int>() { entry.UserID }).FirstOrDefault(i => i.ID == entry.UserID);
                    entry.Author = author != null ? author.Name : string.Empty;

                    var searchParams = new EntrySearchParams(entry.CategoryID) { Status = Statuses.Active };
                    var searchResult = Search(searchParams, true, false);

                    return new EntryDisplayModel()
                    {
                        Entry = entry,
                        RelatedEntries = searchResult.Results.Select(i => new Item(i.ID, i.Name)).ToList(),
                        CategoryHierarchy = _categoryService.GetCategoryHierarchy(entry.CategoryID)
                    };
                }
            }
            return null;
        }

        public Result Save(int userID, Entry model)
        {
            try
            {
                if (model != null && userID > 0)
                {
                    var author = _userService.GetAdminUser(userID);
                    var success = true;
                    if (model.ID > 0 && author != null)
                    {
                        var entry = _entryRepository.First(i => i.ID == model.ID);
                        if (entry == null)
                        {
                            throw new Exception(string.Format("Makale bulunamadı : {0}!", model.ID));
                        }

                        if (author.Role != UserRole.SuperUser && entry.UserID != author.ID)
                        {
                            return Result.Error("Bu makale size ait değil. Güncelleyemezsiniz!");
                        }

                        entry.CategoryID = model.CategoryID;
                        entry.Name = model.Name;
                        entry.Summary = model.Summary;
                        entry.Content = model.Content;
                        entry.Keywords = model.Keywords;
                        entry.ImageInfo = model.ImageInfo;
                        entry.SortOrder = model.SortOrder;
                        entry.Status = author.Role == UserRole.SuperUser ? model.Status : Statuses.WaitingApproval;
                        entry.DateModified = DateTime.Now;

                        success = _entryRepository.Update(entry);
                        model = entry;
                    }
                    else if (author != null)
                    {
                        model.DateModified = DateTime.Now;
                        model.DateCreated = DateTime.Now;
                        model.UserID = userID;
                        model.Status = author.Role == UserRole.SuperUser ? model.Status : Statuses.WaitingApproval;
                        _entryRepository.Add(model);
                    }
                    if (success) { return Result.Success(model, Constants.SuccessMessage); }
                }
            }
            catch (Exception ex)
            {
                _application.Log.Error("EntryService > Save - failed", ex);
            }
            return Result.Error(Constants.ErrorMessage);
        }


        public Result Delete(int userID, int entryID)
        {
            try
            {
                if (userID > 0 && entryID > 0)
                {
                    var entry = _entryRepository.First(i => i.ID == entryID && i.UserID == userID);
                    if (entry != null)
                    {
                        var success = _entryRepository.Delete(entry);
                        if (success) { return Result.Success(null, Constants.DeleteSuccessMessage); }
                    }
                }
            }
            catch (Exception ex)
            {
                _application.Log.Error("EntryService > Delete - failed for user:{0}-id:{1}", ex, userID, entryID);
            }
            return Result.Error(Constants.ErrorMessage);
        }

        private List<User> GetAuthorList(List<int> userIDList)
        {
            var authorList = _application.Cacher.Get<List<User>>(Constants.CK_AuthorList);
            if (userIDList != null && userIDList.Any())
            {
                var idList = userIDList.Where(i => i != 1).Distinct();
                if (authorList != null)
                {
                    var list = authorList;
                    idList = idList.Where(i => list.All(a => a.ID != i));
                }

                if (idList.Any())
                {
                    authorList = authorList ?? new List<User>();
                    authorList.AddRange(_userService.GetUsersByID(idList.ToList()));
                    _application.Cacher.Remove(Constants.CK_AuthorList);
                    _application.Cacher.Add(Constants.CK_AuthorList, authorList);
                }
            }
            return authorList ?? new List<User>();
        }
    }
}