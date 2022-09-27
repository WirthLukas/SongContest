using System.Threading.Tasks;
using SongContest.Core.DataTransferObjects;
using SongContest.Core.Entities;

namespace SongContest.Core.Contracts
{
    public interface IVoteRepository : IGenericRepository<Vote>
    {
        Task<CountryVotesDto[]> GetVotesOfCountryAsync(int countryId);
        Task<(bool SamePoints, bool SameCountry)> IsVoteOfCountryAllowed(int countryId, int points, int votedCountryId);
    }
}
