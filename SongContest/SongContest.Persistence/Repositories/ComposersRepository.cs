using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SongContest.Core.Contracts;
using SongContest.Core.Entities;

namespace SongContest.Persistence.Repositories
{
    internal class ComposersRepository : GenericRepository<ParticipantComposer>, IComposerRepository
    {
        public ComposersRepository(DbContext context) : base(context)
        {
        }

        public Task<string[]> GetComposerNamesOfParticipant(int participantId) =>
            DbSet
                .Where(c => c.ParticipantId == participantId)
                .Select(c => c.Name)
                .ToArrayAsync();
    }
}
