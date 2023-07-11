using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Aware.Data;
using Aware.Dependency;
using Aware.Util.Log;
using CleanFramework.Business.Model;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;

namespace CleanFramework.Data.Repository
{
    public class NhibernateTestRepository
    {
        protected ISession Session { get { return UnitOfWork.Session; } }
        protected ILogger Logger { get { return UnitOfWork.Logger; } }
        protected INhibernateUnitOfWork UnitOfWork
        {
            get { return (INhibernateUnitOfWork)WindsorBootstrapper.Resolve<IUnitOfWork>(); }
        }

        public List<Entry> GetWithPage(int page, int size)
        {
            var guid = UnitOfWork.StartTransaction();
            try
            {
                var criteria = Session.CreateCriteria<Entry>();
                criteria.SetFirstResult((page - 1) * size).SetMaxResults(size);

                var result = criteria.List<Entry>().ToList();
                UnitOfWork.Commit(guid);
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("NhibernateTestRepository > GetWithPage - Failed", ex);
                UnitOfWork.Rollback(guid);
            }
            return null;
        }

        public IList GetEntryListWithCount(int page, int size, out long count)
        {
            var guid = UnitOfWork.StartTransaction();
            count = 0;

            try
            {
                var multiQuery = Session.CreateMultiQuery()
                                           .Add(Session.CreateQuery("from Entry i where i.ID > ?").SetInt32(0, (page-1)*size).SetFirstResult(size))
                                           .Add(Session.CreateQuery("select count(*) from Entry i where i.ID > ?").SetInt32(0, (page - 1) * size));

                var results = multiQuery.List();
                var result = (IList)results[0];
                count = (long)((IList)results[1])[0];

                UnitOfWork.Commit(guid);
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("NhibernateTestRepository > GetEntryListWithCount - Failed", ex);
                UnitOfWork.Rollback(guid);
            }
            return null;
        }

        public IList<Entry> GetPagedData(int page, int size, out long count)
        {
            var guid = UnitOfWork.StartTransaction();
            count = 0;

            try
            {
                var results = Session.CreateMultiCriteria()
                                    .Add(Session.CreateCriteria(typeof(Entry)).SetFirstResult((page-1) * size).SetMaxResults(size))
                                    .Add(Session.CreateCriteria(typeof(Entry)).SetProjection(Projections.RowCountInt64()))
                                    .List();

                var result = ((IList) results[0]).Cast<Entry>().ToList();
                count = (long)((IList)results[1])[0];

                UnitOfWork.Commit(guid);
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("NhibernateTestRepository > GetEntryListWithCount - Failed", ex);
                UnitOfWork.Rollback(guid);
            }
            return null;
        }
    }
}
