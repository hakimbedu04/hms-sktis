using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> GetGenericRepository<T>()
          where T : class;

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// the dispose method is called automatically by the injector depending on the lifestyle
        /// </summary>
        void Dispose();

        /// <summary>
        /// Saves current context changes.
        /// </summary>
        void SaveChanges();

        void RevertChanges();

        ISqlSPRepository GetSPRepository();
    }
}
