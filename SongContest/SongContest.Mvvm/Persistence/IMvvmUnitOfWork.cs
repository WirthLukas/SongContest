using SongContest.Core.Contracts;

namespace SongContest.Mvvm.Persistence
{
    public interface IMvvmUnitOfWork : IUnitOfWork
    {
        void ClearChangeTracker();
        void Reload(IEntityObject entity);
        bool ContextHasChanges { get; }

        void AddEntityToContext(IEntityObject entity);
    }
}
