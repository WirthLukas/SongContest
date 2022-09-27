using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SongContest.Core.Contracts;

namespace SongContest.Persistence.Repositories
{
    /// <summary>
    /// Generische Zugriffsmethoden für eine Entität
    /// Werden spezielle Zugriffsmethoden benötigt, wird eine spezielle
    /// abgeleitete Klasse erstellt.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    internal class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class, IEntityObject, new()
    {
        protected DbSet<TEntity> DbSet { get; } // Set der entsprechenden Entität im Context

        protected DbContext Context { get; }

        public GenericRepository(DbContext context)
        {
            Context = context;
            DbSet = context.Set<TEntity>();
        }

        /// <inheritdoc />
        public async Task<TEntity[]> GetAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params string[] includeProperties)
        {
            IQueryable<TEntity> query = DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            // alle gewünschten abhängigen Entitäten mitladen
            foreach (string includeProperty in includeProperties)
            {
                query = query.Include(includeProperty.Trim());
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToArrayAsync();
            }

            return await query.ToArrayAsync().ConfigureAwait(false);
        }

        /// <inheritdoc />
        public ValueTask<TEntity> GetByIdAsync(int id) => DbSet.FindAsync(id);

        /// <inheritdoc />
        public Task<TEntity> GetByIdAsync(int id, params string[] includeProperties)
        {
            IQueryable<TEntity> query = DbSet;

            foreach (var property in includeProperties)
            {
                query = query.Include(property.Trim());
            }

            return query.SingleOrDefaultAsync(e => e.Id == id);
        }

        public Task<bool> ExistsAsync(int id) => DbSet.AnyAsync(e => e.Id == id);

        /// <inheritdoc />
        public async Task AddAsync(TEntity entity) => await DbSet.AddAsync(entity);

        /// <inheritdoc />
        public Task AddRangeAsync(IEnumerable<TEntity> entities) => DbSet.AddRangeAsync(entities);

        /// <inheritdoc />
        public bool Remove(int id)
        {
            TEntity entityToDelete = DbSet.Find(id);

            if (entityToDelete != null)
            {
                Remove(entityToDelete);
                return true;
            }
            
            return false;
        }

        /// <inheritdoc />
        public void Remove(TEntity entityToRemove)
        {
            if (Context.Entry(entityToRemove).State == EntityState.Detached)
            {
                DbSet.Attach(entityToRemove);
            }
            DbSet.Remove(entityToRemove);
        }

        ///// <summary>
        /////     Entität aktualisieren
        ///// </summary>
        ///// <param name="entityToUpdate"></param>
        //public void Update(TEntity entityToUpdate)
        //{
        //    //Prüfen ob Entität bereits im aktuellen Context vorhanden (falls ja, muss vorher Detach auf Entität durchgeführt werden,
        //    //um Attach ausführen zu können)
        //    TEntity localEntity = DbSet.Local.FirstOrDefault(x => x.Id == entityToUpdate.Id);
        //    if (localEntity != null)
        //    {
        //        if (Context.Entry(entityToUpdate).State == EntityState.Added)
        //        {
        //            throw new DbUpdateException("Update performed on inserted but not commited dataset", default(Exception));
        //        }
        //        Context.Entry(localEntity).State = EntityState.Added;
        //        DbSet.Local.Remove(localEntity);
        //    }
        //    DbSet.Attach(entityToUpdate);
        //    Context.Entry(entityToUpdate).State = EntityState.Modified;
        //    //Context.Update(entityToUpdate);
        //}

        /// <inheritdoc />
        public Task<int> CountAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> query = DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query.CountAsync();
        }
    }
}
