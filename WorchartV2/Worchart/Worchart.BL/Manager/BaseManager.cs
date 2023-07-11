using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using Worchart.BL.Model;
using Worchart.BL.Search;
using Worchart.Data;
using Worchart.BL.Log;
using Worchart.BL.Constants;
using Worchart.BL.Enum;
using Worchart.BL.Cache;

namespace Worchart.BL.Manager
{
    public abstract class BaseManager<T> : IBaseManager<T> where T : IEntity
    {
        protected readonly IRepository<T> Repository;
        protected readonly ILogger Logger;
        protected readonly ICacher Cacher;
        private readonly string _typeName;

        protected BaseManager(IRepository<T> repository, ILogger logger)
        {
            Repository = repository;
            Logger = logger;
            Cacher = null;
            _typeName = typeof(T).Name;
        }

        protected BaseManager(IRepository<T> repository, ILogger logger, ICacher cacher)
        {
            Repository = repository;
            Logger = logger;
            Cacher = cacher;
            _typeName = typeof(T).Name;
        }

        public T Get(int id)
        {
            try
            {
                if (id > 0)
                {
                    if (CacheMode != ManagerCacheMode.UseCache)
                    {
                        return First(i => i.ID == id);
                    }
                    else
                    {
                        return Repository.Get(id);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("BaseManager|Get<T>", "T:{0}, ID:{1}", ex, typeof(T), id);
            }
            return default(T);
        }

        public T First(Expression<Func<T, bool>> filter = null, T defaultValue = default(T))
        {
            try
            {
                var searchParams = new SearchParams<T>();
                searchParams.AddFilter(filter);
                searchParams.SetPaging(1, 1);

                var searchResult = Search(searchParams);
                if (searchResult != null && searchResult.HasResult) { return searchResult.Results.FirstOrDefault(); }
            }
            catch (Exception ex)
            {
                Logger.Error("BaseManager|SearchBy<T>", "T:{0}", ex, _typeName);
            }
            return default(T);
        }

        public SearchResult<T> SearchBy(Expression<Func<T, bool>> filter = null, int page = 1, int pageSize = 0)
        {
            try
            {
                var searchParams = new SearchParams<T>();
                searchParams.AddFilter(filter);
                searchParams.SetPaging(page, pageSize);
                return Search(searchParams);
            }
            catch (Exception ex)
            {
                Logger.Error("BaseManager|SearchBy<T>", "T:{0}", ex, _typeName);
            }
            return null;
        }

        public SearchResult<T> Search(SearchParams<T> searchParams = null)
        {
            try
            {
                if (searchParams != null)
                {
                    if (CacheMode != ManagerCacheMode.NoCache)
                    {
                        var list = Cacher.Get<List<T>>(CacheKey);
                        if (list == null)
                        {
                            list = Repository.GetAll();
                            Cacher.Add(CacheKey, list, CommonConstants.DailyCacheTime);
                        }

                        if (list != null)
                        {
                            var queryable = list.AsQueryable();
                            var searchHelper = searchParams.PrepareFilters();
                            if (searchHelper.FilterList != null && searchHelper.FilterList.Any())
                            {
                                foreach (var filter in searchHelper.FilterList)
                                {
                                    queryable = queryable.Where(filter);
                                }
                            }

                            if (searchHelper.SortList != null && searchHelper.SortList.Any())
                            {
                                foreach (var sorter in searchHelper.SortList)
                                {
                                    sorter.OrderBy(ref queryable);
                                }
                            }

                            var totalSize = searchParams.IncludeCount ? queryable.Count() : 0;
                            if (searchParams.Size > 0)
                            {
                                queryable = queryable.Skip((searchParams.Page - 1) * searchParams.Size).Take(searchParams.Size);
                            }

                            return new SearchResult<T>
                            {
                                Results = queryable.ToList(),
                                SearchParams = searchParams,
                                TotalSize = totalSize
                            };
                        }
                    }
                    else
                    {
                        return Repository.Find(searchParams);
                    }
                }
                else
                {
                    return new SearchResult<T>
                    {
                        Results = Repository.GetAll(),
                        SearchParams = searchParams
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.Error("BaseManager|Search<T>", "T:{0}", ex, _typeName);
            }
            return null;
        }

        public OperationResult Save(T model)
        {
            OperationResult result = null;
            try
            {
                if (model != null)
                {
                    var isUpdate = model.ID > 0;
                    if (isUpdate)
                    {
                        var existing = Repository.Where(i => i.ID == model.ID).First();
                        if (existing != null)
                        {
                            result = OnBeforeUpdate(ref existing, model);
                            if (result.Success)
                            {
                                Repository.Update(existing);
                                model = existing;
                            }
                        }
                    }
                    else
                    {
                        result = OnBeforeCreate(ref model);
                        if (result.Success)
                        {
                            Repository.Add(model);
                        }
                    }

                    if (result != null && result.Success)
                    {
                        OnAfterSave(model);
                        if (CacheMode == ManagerCacheMode.UseResponsiveCache)
                        {
                            RefreshCache(model, isUpdate ? 1 : 0);
                        }
                        result.Value = model;
                    }
                }
            }
            catch (Exceptions.WorchartException wex)
            {
                Logger.Error("BaseManager|Save<T>", "T:{0}, ID:{1}", wex, _typeName, model != null ? model.ID : 0);
                return Failed(wex.Code);
            }
            catch (Exception ex)
            {
                Logger.Error("BaseManager|Save<T>", "T:{0}, ID:{1}", ex, _typeName, model != null ? model.ID : 0);
                result = null;
            }
            return result ?? Failed();
        }

        public OperationResult Clone(int id, bool create = false)
        {
            try
            {
                if (id > 0)
                {
                    var model = Get(id);
                    if (model != null)
                    {
                        var clone = GetClone(model);
                        if (create)
                        {
                            return Save(clone);
                        }
                        return Success(clone);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("BaseManager|Clone<T>", "T:{0}, ID:{1}", ex, _typeName, id);
            }
            return Failed();
        }

        public OperationResult Delete(int id)
        {
            try
            {
                if (id > 0)
                {
                    var result = OnBeforeDelete(id);
                    if (result.Success)
                    {
                        var success = Repository.Delete(id);
                        if (!success)
                        {
                            result.Code = ErrorConstants.OperationFailed;
                        }

                        if (success && CacheMode == ManagerCacheMode.UseResponsiveCache)
                        {
                            RefreshCache(default(T), 2, id);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("BaseManager|Delete<T>", "T:{0}, ID:{1}", ex, _typeName, id);
            }
            return Failed();
        }

        protected abstract OperationResult OnBeforeUpdate(ref T existing, T model);
        protected virtual OperationResult OnBeforeCreate(ref T model) { return Success(); }
        protected virtual OperationResult OnBeforeDelete(int id) { return Success(); }
        protected virtual void OnAfterSave(T model) { }
        protected virtual T GetClone(T model)
        {
            if (model != null)
            {
                var methodInfo = model.GetType().GetMethod("Clone");
                var result = (T)methodInfo.Invoke(model, null);
                if (result != null)
                {
                    result.ID = 0;
                    return result;
                }
            }
            return default(T);
        }

        protected OperationResult Success(object value = null, string message = "")
        {
            return new OperationResult(ErrorConstants.OperationSuccess) { Message = message, Value = value };
        }

        protected OperationResult Failed(string message = "", string code = ErrorConstants.OperationFailed)
        {
            return new OperationResult(code) { Message = message };
        }

        protected virtual ManagerCacheMode CacheMode { get { return ManagerCacheMode.NoCache; } }
        protected string CacheKey
        {
            get
            {
                return string.Format("ENTITY.CACHE.{0}", typeof(T).Name.ToUpperInvariant());
            }
        }

        private bool RefreshCache(T model, int operation, int id = 0)
        {
            var list = Cacher.Get<List<T>>(CacheKey);
            if (list != null)
            {
                if (operation == 1) { list = list.Where(i => i.ID != model.ID).ToList(); }
                else if (operation == 2) { list = list.Where(i => i.ID != id).ToList(); }

                if (operation == 0 || operation == 1) { list.Add(model); }
                Cacher.Remove(CacheKey);
                Cacher.Add(CacheKey, list, CommonConstants.DailyCacheTime);
                return true;
            }
            return false;
        }
    }
}
