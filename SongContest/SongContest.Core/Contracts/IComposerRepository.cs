using System.Threading.Tasks;
using SongContest.Core.Entities;

namespace SongContest.Core.Contracts
{
    public interface IComposerRepository: IGenericRepository<ParticipantComposer>
    {
        Task<string[]> GetComposerNamesOfParticipant(int participantId);
    }
}
