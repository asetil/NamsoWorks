using System;
using System.Collections.Generic;
using System.Data.Entity;
using NHibernate;
using System.Data;
using Aware.Util.Log;

namespace Aware.Data.NHibernate
{
    public class UnitOfWork : IDisposable, INhibernateUnitOfWork
    {
        private ISession _session;
        private ITransaction _transaction;
        private Guid _transactionGuid;
        private bool _isRollbacked;

        private readonly ISessionFactory _sessionFactory;
        private readonly ILogger _logger;

        public UnitOfWork(ISessionFactory sessionFactory, ILogger logger)
        {
            _sessionFactory = sessionFactory;
            _logger = logger;
            _transactionGuid = Guid.Empty;
        }

        public ISession Session
        {
            get { return _session; }
        }

        public ILogger Logger
        {
            get
            {
                return _logger;
            }
        }

        public Guid StartTransaction()
        {
            if (_transactionGuid == Guid.Empty)
            {
                //Open new session
                _session = _sessionFactory.OpenSession();
                _session.FlushMode = FlushMode.Commit;

                //Start new transaction
                _transaction = _session.BeginTransaction(IsolationLevel.ReadUncommitted);
                _transactionGuid = Guid.NewGuid();
                _isRollbacked = false;
                return _transactionGuid;
            }
            return Guid.Empty;
        }

        public bool Commit(Guid transactionGuid)
        {
            try
            {
                //Daha önce rollback edilmiş, commit edemeyiz!
                var isValid=IsValidGuid(transactionGuid);
                if (isValid && _isRollbacked)
                {
                    return Rollback(transactionGuid);
                }

                if (isValid && _transaction != null && _transaction.IsActive)
                {
                    _transaction.Commit();
                    _transactionGuid = Guid.Empty;
                    _transaction = null;

                    if (Session != null)
                    {
                        Session.Dispose();
                        _session = null;
                    }
                }
                return _transactionGuid == Guid.Empty;
            }
            catch
            {
                if (_transaction != null && _transaction.IsActive)
                {
                    _isRollbacked = true;
                    Rollback(transactionGuid);
                }
                throw;
            }
        }

        public bool Rollback(Guid transactionGuid)
        {
            try
            {
                if (IsValidGuid(transactionGuid))
                {
                    if (_transaction != null && _transaction.IsActive)
                    {
                        _transaction.Rollback();
                        _transaction = null;

                        if (Session != null)
                        {
                            Session.Dispose();
                            _session = null;
                        }
                    }

                    _transactionGuid = Guid.Empty;
                    return true;
                }
                else
                {
                    _isRollbacked = true;
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool IsValidGuid(Guid transactionGuid)
        {
            return _transactionGuid != Guid.Empty && _transactionGuid == transactionGuid;
        }

        private bool disposed = false;
        public void Dispose()
        {
            if (!disposed)
            {
                if (_transactionGuid != Guid.Empty)
                {
                    Rollback(_transactionGuid);
                }

                _transaction = null;
                if (_session != null) { _session.Dispose(); }
                _session = null;
            }

            disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}