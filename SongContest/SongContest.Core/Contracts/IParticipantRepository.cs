using System.Threading.Tasks;
using SongContest.Core.DataTransferObjects;
using SongContest.Core.Entities;

namespace SongContest.Core.Contracts
{
    public interface IParticipantRepository : IGenericRepository<Participant>
    {
        Task<Participant> GetParticipantByCountry(int countryId);
        Task<Participant[]> GetNonVotedParticipantsForCountry(int countryId);
        Task<Participant> GetParticipantWithMost12Points();
        Task<ParticipantResultsDto[]> GetParticipantResults();
    }
}
