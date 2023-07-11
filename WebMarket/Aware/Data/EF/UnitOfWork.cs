using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using Aware.Util.Log;

namespace Aware.Data.EF
{
    public class UnitOfWork : IDisposable, IEFUnitOfWork
    {
        private readonly ILogger _logger;
        private static ECommerceEntities _context;
        private Dictionary<string, object> repositories;
        private DbContextTransaction _transaction;
        private Guid _transactionGuid;
        private bool _isRollbacked;

        public UnitOfWork(ILogger logger)
        {
            _logger = logger;
            _context = new ECommerceEntities();
            _transactionGuid = Guid.Empty;
        }

        public DbContext Context
        {
            get { return _context ?? (_context = new ECommerceEntities()); }
        }

        public void Save()
        {
            Context.SaveChanges();
        }

        private void SaveChanges()
        {
            try
            {
                IEnumerable<DbEntityValidationResult> validationResultList = Context.GetValidationErrors();
                if (validationResultList.Count() > 0)
                {
                    foreach (var validationResult in validationResultList)
                    {
                        foreach (var validationError in validationResult.ValidationErrors)
                        {
                            throw new Exception(string.Format("PropertyName : {0}, ErrorMessage : {1}",
                                validationError.PropertyName, validationError.ErrorMessage));
                        }
                    }

                }

                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Guid StartTransaction()
        {
            if (_transactionGuid == Guid.Empty && _transaction == null && Context != null)
            {
                _transaction = Context.Database.BeginTransaction();
                _transactionGuid = Guid.NewGuid();
                return _transactionGuid;
            }
            return Guid.Empty;
        }

        public bool Commit(Guid transactionGuid)
        {
            try
            {
                //Daha önce rollback edilmiş, commit edemeyiz!
                if (_transactionGuid != Guid.Empty && _isRollbacked)
                {
                    return Rollback(transactionGuid);
                }

                if (transactionGuid != Guid.Empty && _transaction != null)
                {
                    _transaction.Commit();
                    _transactionGuid = Guid.Empty;
                    _transaction = null;
                    return true;
                }
                return transactionGuid == Guid.Empty;
            }
            catch
            {
                if (_transaction != null)
                    _isRollbacked = true;
                throw;
            }
        }

        public bool Rollback(Guid transactionGuid)
        {
            try
            {
                if (transactionGuid != Guid.Empty && _transaction != null)
                {
                    if (_transaction != null)
                    {
                        _transaction.Rollback();
                        _transactionGuid = Guid.Empty;

                        foreach (var entry in _context.ChangeTracker.Entries())
                        {
                            if (entry.State != EntityState.Unchanged)
                                entry.State = EntityState.Unchanged;
                        }
                        return true;
                    }
                }
                else if (transactionGuid == Guid.Empty)
                {
                    _isRollbacked = true;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool disposed = false;
        public void Dispose()
        {
            if (!disposed)
            {
                Context.Dispose();
                _context = null;
            }

            disposed = true;
            GC.SuppressFinalize(this);
        }

        public ILogger Logger { get { return _logger; } }
    }
}