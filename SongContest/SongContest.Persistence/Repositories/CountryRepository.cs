using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SongContest.Core.Contracts;
using SongContest.Core.DataTransferObjects;
using SongContest.Core.Entities;

namespace SongContest.Persistence.Repositories
{
    internal class CountryRepository : GenericRepository<Country>, ICountryRepository
    {
        private ApplicationDbContext DbContext => (ApplicationDbContext) Context;

        public CountryRepository(DbContext context) : base(context)
        {
        }

        // Diese Version bietet ungefähr die selbe performance wie de untere Methode
        //public async Task<CountryStatisticsDto[]> GetCountryStatisticsAsync()
        //{
        //    var countryVotes = await DbContext.Votes
        //        .GroupBy(v => new { Id = v.CountryId, Name = v.Country.Name })
        //        .Select(grp => new
        //        {
        //            CountryId = grp.Key.Id,
        //            Country = grp.Key.Name,
        //            GivenVotes = grp.Count(v => v.Points > 0)
        //        })
        //        .OrderBy(item => item.Country)
        //        .ToArrayAsync();

        //    var countryPoints = await DbContext.Votes
        //        .GroupBy(v => v.Participant.Country.Name)
        //        .Select(grp => new
        //        {
        //            Country = grp.Key,
        //            Points = grp.Sum(v => v.Points)
        //        })
        //        .ToArrayAsync();

        //    return countryVotes
        //        .Select(item => new CountryStatisticsDto(
        //            Id: item.CountryId,
        //            Country: item.Country,
        //            GivenVotes: item.GivenVotes,
        //            Points: countryPoints.SingleOrDefault(x => x.Country == item.Country)?.Points
        //        ))
        //        .ToArray();
        //}

        // DB Context aufrufe werden in Subqueries umgewandelt
        public Task<CountryStatisticsDto[]> GetCountryStatisticsAsync()
        {
            return DbContext.Countries
                .OrderBy(country => country.Name)
                .Select(country => new CountryStatisticsDto(
                    country.Id,
                    country.Name,
                    DbContext.Votes.Where(v => v.CountryId == country.Id).Count(vote => vote.Points > 0),
                    (
                        DbContext.Participants.Any(p => p.CountryId == country.Id)
                            ? DbContext.Votes
                                .Where(vote => vote.Participant.CountryId == country.Id)
                                .Sum(vote => vote.Points)
                            : (int?)null
                    )
                ))
                .ToArrayAsync();
        }

        public Task<Country[]> GetVotableCountries() =>
            DbContext.Participants
                .Select(p => p.Country)
                .OrderBy(c => c.Name)
                .ToArrayAsync();
    }
}
