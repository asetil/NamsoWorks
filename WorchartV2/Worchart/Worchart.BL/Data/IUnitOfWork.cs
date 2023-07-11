using NHibernate;
using System;
using Worchart.BL.Log;

namespace Worchart.Data
{
    public interface IUnitOfWork
    {
        ISession GetCurrentSession();
        Guid StartTransaction();
        bool Commit(Guid transactionGuid);
        bool Rollback(Guid transactionGuid);
        void Dispose();
    }
}