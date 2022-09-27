using Microsoft.EntityFrameworkCore;
using SongContest.Core.Contracts;
using SongContest.Persistence;

namespace SongContest.Mvvm.Persistence
{
    internal class MvvmUnitOfWork : UnitOfWork, IMvvmUnitOfWork
    {
        public bool ContextHasChanges => DbContext.ChangeTracker.HasChanges();

        public void ClearChangeTracker() => DbContext.ChangeTracker.Clear();

        public void Reload(IEntityObject entity) => DbContext.Entry(entity).Reload();

        public void AddEntityToContext(IEntityObject entity)
        {
            DbContext.Attach(entity);
            DbContext.Entry(entity).State = EntityState.Added;
        }
    }
}
