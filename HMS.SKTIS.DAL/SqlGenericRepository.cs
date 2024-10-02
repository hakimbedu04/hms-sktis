using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.Contracts;

namespace HMS.SKTIS.DAL
{
    public class SqlGenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        internal SKTISEntities context;
        internal DbSet<TEntity> dbSet;
        public SqlGenericRepository(SKTISEntities contextEntities)
        {
            this.context = contextEntities;
            this.dbSet = context.Set<TEntity>();
        }

        public void Insert(TEntity entity)
        {
            dbSet.Add(entity);
        }

        public void Update(TEntity entityToUpdate)
        {
            var entry = context.Entry<TEntity>(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public void InsertOrUpdate(TEntity entity)
        {
            if (!Exists(entity))
                Insert(entity);
            else
                Update(entity);
        }

        public bool Exists(TEntity entity)
        {
            var objContext = ((IObjectContextAdapter)this.context).ObjectContext;
            var objSet = objContext.CreateObjectSet<TEntity>();
            var entityKey = objContext.CreateEntityKey(objSet.EntitySet.Name, entity);

            Object foundEntity;
            var exists = objContext.TryGetObjectByKey(entityKey, out foundEntity);
            return (exists);
        }

        public void Delete(object id)
        {
            TEntity entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
        }

        public void Delete(TEntity entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }

        public void DeleteAll()
        {
            dbSet.RemoveRange(dbSet);
        }

        public TEntity GetByID(object id)
        {
            return dbSet.Find(id);
        }

        public TEntity GetByID(params object[] keyValues)
        {
            return dbSet.Find(keyValues);
        }

        public IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }
    }
}
