using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Aware.Model;
using Aware.Search;
using Microsoft.EntityFrameworkCore;

namespace Aware.Data.EF
{
    public class EFRepository<T> : IRepository<T> where T : class, IEntity
    {
        private readonly IDbContextFactory _dbContextFactory;
        public EFRepository(IDbContextFactory factory)
        {
            _dbContextFactory = factory;
        }

        public T Get(int id)
        {
            return DbSet.Find(id);
        }

        public List<T> GetAll()
        {
            return DbSet.ToList();
        }

        public T First(Expression<Func<T, bool>> filter, bool last = false)
        {
            if (filter != null)
            {
                IQueryable<T> query = DbSet;
                query = query.Where(filter);

                if (last)
                {
                    return query.LastOrDefault();
                }
                return query.FirstOrDefault();
            }
            return default(T);
        }

        public SearchResult<T> Find(ISearchParams<T> searchParams)
        {
            var result = new SearchResult<T>();
            IQueryable<T> query = DbSet;
            if (searchParams != null)
            {
                if (searchParams.FilterList != null && searchParams.FilterList.Any())
                {
                    foreach (var filter in searchParams.FilterList)
                    {
                        query = query.Where(filter);
                    }
                }

                if (!string.IsNullOrEmpty(searchParams.NavigationFields))
                {
                    foreach (var field in searchParams.NavigationFields.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Include(field);
                    }
                }

                if (searchParams.SortList != null && searchParams.SortList.Any())
                {
                    foreach (var sorter in searchParams.SortList)
                    {
                        sorter.OrderBy(ref query);
                    }
                }

                if (searchParams.Size > 0)
                {
                    query = query.Skip((searchParams.Page - 1) * searchParams.Size).Take(searchParams.Size);
                }

                result.Results = query.ToList();
                result.TotalSize = GetCount(searchParams.FilterList);
                result.Success = true;
                result.SearchParams = searchParams;
            }
            return result;
        }

        public void Add(T entity, bool save = true)
        {
            DbSet.Add(entity);

            if (save)
            {
                Save();
            }
        }

        public bool Update(T entity, bool save = true)
        {
            if (Context.Entry(entity).State == EntityState.Detached)
            {
                Context.Set<T>().Attach(entity);
            }
            Context.Entry(entity).State = EntityState.Modified;

            if (save)
            {
                Save();
            }
            return true;
        }

        public bool Delete(int id)
        {
            var entity = DbSet.Find(id);
            return Delete(entity);
        }

        public bool Delete(T entity)
        {
            if (Context.Entry(entity).State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }
            DbSet.Remove(entity);
            return true;
        }

        public int ExecuteSp(string storedProcedureQuery)
        {
            throw new NotImplementedException();
        }

        public int GetCount(List<Expression<Func<T, bool>>> filters)
        {
            IQueryable<T> query = DbSet;
            if (filters != null && filters.Any())
            {
                foreach (var filter in filters)
                {
                    query = query.Where(filter);
                }
            }
            return query.Count();
        }

        public List<T> GetWithSql(string query, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public List<TX> GetWithSql<TX>(string query, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            Context.SaveChanges();
        }

        private bool _disposed = false;
        public void Dispose()
        {
            if (!_disposed)
            {
                Context.Dispose();
            }
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        private DbContext Context
        {
            get
            {
                return _dbContextFactory.GetDbContext();
            }
        }

        private DbSet<T> DbSet
        {
            get
            {
                return _dbContextFactory.GetDbSet<T>();
            }
        }
    }
}
