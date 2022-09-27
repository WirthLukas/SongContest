using System.Threading.Tasks;
using SongContest.Core.DataTransferObjects;
using SongContest.Core.Entities;

namespace SongContest.Core.Contracts
{
    public interface ICountryRepository : IGenericRepository<Country>
    {
        Task<Country[]> GetVotableCountries();
        Task<CountryStatisticsDto[]> GetCountryStatisticsAsync();
    }
}
