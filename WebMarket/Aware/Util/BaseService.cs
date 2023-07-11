using System;
using System.Collections.Generic;
using Aware.Data;
using Aware.ECommerce.Model;
using Aware.Search;
using Aware.Util.Log;
using Aware.Util.Model;

namespace Aware.Util
{
    public abstract class BaseService<T> : IBaseService<T> where T : class, IEntity
    {
        protected readonly IRepository<T> Repository;
        protected readonly ILogger Logger;
        private readonly string _typeName;

        protected BaseService(IRepository<T> repository, ILogger logger)
        {
            Repository = repository;
            Logger = logger;
            _typeName = typeof(T).Name;
        }

        public T Get(int id)
        {
            if (id > 0)
            {
                return Repository.Get(id);
            }
            return default(T);
        }

        public List<T> GetAll(int page = 1, int pageSize = 20)
        {
            return Repository.Where(i => i.ID > 0).SetPaging(page, pageSize).ToList();
        }

        public SearchResult<T> Search(SearchParams<T> searchParams)
        {
            try
            {
                if (searchParams != null)
                {
                    return Repository.Find(searchParams);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(_typeName + "Service > Search - failed", ex);
            }
            return null;
        }

        public Result Save(T model)
        {
            try
            {
                if (model != null)
                {
                    if (model.ID > 0)
                    {
                        var existing = Repository.Where(i => i.ID == model.ID).First();
                        if (existing != null)
                        {
                            OnBeforeUpdate(ref existing, model);
                            Repository.Update(existing);
                            model = existing;
                        }
                    }
                    else
                    {
                        OnBeforeCreate(ref model);
                        Repository.Add(model);
                    }

                    OnAfterSave(model);
                    return Result.Success(model, Resource.General_Success);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(_typeName + "ServiceService > Save - Fail with ID:{0}", ex, model != null ? model.ID : 0);
            }
            return Result.Error(Resource.General_Error);
        }

        public Result Delete(int id)
        {
            Result result = null;
            try
            {
                if (id > 0)
                {
                    result = OnBeforeDelete(id);
                    if (result == null || result.OK)
                    {
                        Repository.Delete(id);
                        result = Result.Success(null, Resource.General_Success);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(_typeName + "ServiceService > Delete - faield with : {0}!", ex, id);
            }
            return result ?? Result.Error(Resource.General_Error);
        }

        protected abstract void OnBeforeUpdate(ref T existing, T model);
        protected virtual void OnBeforeCreate(ref T model) { }
        protected virtual Result OnBeforeDelete(int id) { return null; }
        protected virtual void OnAfterSave(T model) { }

    }
}
