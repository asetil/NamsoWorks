using System;
using System.Data.Entity;
using Aware.Util.Log;
using NHibernate;

namespace Aware.Data
{
    public interface IUnitOfWork
    {
        ILogger Logger { get; }
        Guid StartTransaction();
        bool Commit(Guid transactionGuid);
        bool Rollback(Guid transactionGuid);
        void Dispose();
    }

    public interface INhibernateUnitOfWork : IUnitOfWork
    {
        ISession Session { get; }
    }

    public interface IEFUnitOfWork : IUnitOfWork
    {
        DbContext Context { get; }
        void Save();
    }
}