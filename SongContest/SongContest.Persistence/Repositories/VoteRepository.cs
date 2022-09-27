using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SongContest.Core.Contracts;
using SongContest.Core.DataTransferObjects;
using SongContest.Core.Entities;

namespace SongContest.Persistence.Repositories
{
    internal class VoteRepository : GenericRepository<Vote>, IVoteRepository
    {
        public VoteRepository(DbContext context) : base(context)
        {
        }

        public Task<CountryVotesDto[]> GetVotesOfCountryAsync(int countryId)
        {
            return DbSet
                .Where(vote => vote.CountryId == countryId)
                .OrderByDescending(v => v.Points)
                .ThenBy(v => v.Participant.Country.Name)
                .Select(vote => new CountryVotesDto(vote.Id, vote.Participant.Country.Name, vote.Points))
                .ToArrayAsync();
        }

        public async Task<(bool SamePoints, bool SameCountry)> IsVoteOfCountryAllowed(int countryId, int points, int votedCountryId)
        {
            //var result = await DbSet
            //    .Where(vote => vote.CountryId == countryId)
            //    .GroupBy(v => v.CountryId)
            //    .Select(grp => new
            //    {
            //        SamePoints = grp.Any(v => points != 0 && v.Points == points),
            //        //SameCountry = grp.Any(vote => vote.Participant.CountryId == votedCountryId)
            //        SameCountry = false
            //    })
            //    .SingleAsync();

            bool samePoints = points != 0 && await DbSet
                .Where(vote => vote.CountryId == countryId)
                .Where(vote => vote.Points != 0)
                .AnyAsync(v => v.Points == points);

            bool sameCountry = await DbSet
                .Where(vote => vote.CountryId == countryId)
                .AnyAsync(vote => vote.Participant.CountryId == votedCountryId);

            return (samePoints, sameCountry);
        }
    }
}
