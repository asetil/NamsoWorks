using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using Aware.Model;
using Aware.Data;
using Aware.Util.Enum;
using Aware.Search;
using Aware.Util.Constants;
using Aware.BL.Model;
using Aware.Util.Exceptions;
using Aware.Util.Log;
using Aware.Util.Cache;

namespace Aware.Manager
{
    public abstract class BaseManager<T> : IBaseManager<T> where T : BaseEntity
    {
        protected readonly IRepository<T> Repository;
        protected readonly IAwareLogger Logger;
        protected readonly IAwareCacher Cacher;
        private readonly string _typeName;

        protected BaseManager(IRepository<T> repository, IAwareLogger logger)
        {
            Repository = repository;
            Logger = logger;
            Cacher = null;
            _typeName = typeof(T).Name;
        }

        protected BaseManager(IRepository<T> repository, IAwareLogger logger, IAwareCacher cacher)
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
                searchParams.FilterBy(filter).SetPaging(1, 1);

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
                searchParams.FilterBy(filter).SetPaging(page, pageSize);
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
                            if (searchParams.FilterList != null && searchParams.FilterList.Any())
                            {
                                foreach (var filter in searchParams.FilterList)
                                {
                                    queryable = queryable.Where(filter);
                                }
                            }

                            if (searchParams.SortList != null && searchParams.SortList.Any())
                            {
                                foreach (var sorter in searchParams.SortList)
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

        public OperationResult<T> Save(T model)
        {
            OperationResult<T> result = null;
            try
            {
                if (model != null)
                {
                    var isUpdate = model.ID > 0;
                    if (isUpdate)
                    {
                        var existing = Repository.Get(model.ID);
                        if (existing != null)
                        {
                            existing.DateModified = DateTime.Now;
                            existing.UserModified = CurrentUserID;

                            result = OnBeforeUpdate(ref existing, model);
                            if (result.Ok)
                            {
                                Repository.Update(existing);
                                model = existing;
                            }
                        }
                    }
                    else
                    {
                        model.Status = StatusType.Active;
                        model.DateCreated = DateTime.Now;
                        model.UserCreated = CurrentUserID;
                        model.DateModified = DateTime.Now;
                        model.UserModified = CurrentUserID;

                        result = OnBeforeCreate(ref model);
                        if (result.Ok)
                        {
                            Repository.Add(model);
                        }
                    }

                    if (result != null && result.Ok)
                    {
                        OnAfterSave(model);
                        if (CacheMode == ManagerCacheMode.UseResponsiveCache)
                        {
                            RefreshCache(model, isUpdate ? 1 : 0);
                        }
                        result.SetValue(model);
                    }
                }
            }
            catch (AwareException aex)
            {
                Logger.Error("BaseManager|Save<T>", "T:{0}, ID:{1}", aex, _typeName, model != null ? model.ID : 0);
                return OperationResult<T>.Error(aex.Code);
            }
            catch (Exception ex)
            {
                Logger.Error("BaseManager|Save<T>", "T:{0}, ID:{1}", ex, _typeName, model != null ? model.ID : 0);
                result = null;
            }
            return result ?? OperationResult<T>.Error(ResultCodes.Error.OperationFailed);
        }

        public OperationResult<T> Clone(int id, bool create = false)
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
                        return OperationResult<T>.Success(clone);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("BaseManager|Clone<T>", "T:{0}, ID:{1}", ex, _typeName, id);
            }
            return OperationResult<T>.Error(ResultCodes.Error.OperationFailed);
        }

        public OperationResult<T> Delete(int id)
        {
            try
            {
                if (id > 0)
                {
                    var result = OnBeforeDelete(id);
                    if (result.Ok)
                    {
                        var success = Repository.Delete(id);
                        if (!success)
                        {
                            result = OperationResult<T>.Error(ResultCodes.Error.OperationFailed);
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
            return OperationResult<T>.Error(ResultCodes.Error.OperationFailed);
        }

        protected abstract OperationResult<T> OnBeforeUpdate(ref T existing, T model);

        protected virtual OperationResult<T> OnBeforeCreate(ref T model) { return OperationResult<T>.Success(); }

        protected virtual OperationResult<T> OnBeforeDelete(int id) { return OperationResult<T>.Success(); }

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

        protected OperationResult<T> Success(T value = default(T))
        {
            return OperationResult<T>.Success(value);
        }

        protected OperationResult<T> Failed(string errorCode)
        {
            return OperationResult<T>.Error(errorCode);
        }

        protected OperationResult<TX> Success<TX>(TX value = default(TX))
        {
            return OperationResult<TX>.Success(value);
        }

        protected OperationResult<TX> Failed<TX>(string errorCode)
        {
            return OperationResult<TX>.Error(errorCode);
        }

        protected int CurrentUserID
        {
            get
            {
                return 0;
            }
        }
    }
}
