using System;
using System.Threading.Tasks;

namespace SongContest.Core.Contracts
{
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        IVoteRepository Votes { get; }
        IComposerRepository Composers { get; }
        ICountryRepository Countries { get; }
        IParticipantRepository Participants { get; }

        Task<int> SaveChangesAsync();
        Task DeleteDatabaseAsync();
        Task MigrateDatabaseAsync();
        Task CreateDatabaseAsync();


    }
}
