using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.Contracts;

namespace HMS.SKTIS.DAL
{
    public class SqlUnitOfWork : IUnitOfWork
    {
        private SKTISEntities _context = new SKTISEntities();

        public SqlUnitOfWork()
        {
        }

        public IGenericRepository<T> GetGenericRepository<T>() where T : class
        {
            return new SqlGenericRepository<T>(_context);
        }
       
        public void SaveChanges()
        {
            try
            {
                _context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                throw;
            }
        }

        public void RevertChanges()
        {
            _context = new SKTISEntities();
        }

        private bool disposed = false;

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// the dispose method is called automatically by the injector depending on the lifestyle
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private SqlSPRepository _SPRepository;

        public ISqlSPRepository GetSPRepository()
        {
            if (this._SPRepository == null)
                this._SPRepository = new SqlSPRepository(_context);

            return _SPRepository;
        }
    }
}
