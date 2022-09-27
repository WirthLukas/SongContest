using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SongContest.Core.Contracts;
using SongContest.Core.DataTransferObjects;
using SongContest.Core.Entities;

namespace SongContest.Persistence.Repositories
{
    internal class ParticipantRepository : GenericRepository<Participant>, IParticipantRepository
    {
        private ApplicationDbContext DbContext => (ApplicationDbContext) Context;

        public ParticipantRepository(DbContext context) : base(context)
        {
        }

        public Task<Participant> GetParticipantByCountry(int countryId) =>
            DbSet
                .FirstOrDefaultAsync(p => p.CountryId == countryId);

        public Task<Participant[]> GetNonVotedParticipantsForCountry(int countryId)
        {
            //return DbContext.Votes
            //    .GroupBy(v => v.Participant.CountryId)
            //    .Where(grp => grp.All(v => v.CountryId != countryId))
            //    .SelectMany(grp => grp.Select(v => v.Participant))
            //    .ToArrayAsync();

            //var result = await DbContext.Votes
            //    .GroupBy(v => v.Participant.CountryId)
            //    .Where(grp => grp.All(v => v.CountryId != countryId))
            //    .Select(grp => grp.First().Participant)
            //    .ToArrayAsync();

            return DbContext.Participants
                .Where(p => p.Votes.All(v => v.CountryId != countryId))
                .ToArrayAsync();
        }

        public Task<Participant> GetParticipantWithMost12Points() =>
            DbSet
                .Select(p => new
                {
                    Participant = p,
                    Count = p.Votes.Count(v => v.Points == 12)
                })
                .OrderByDescending(item => item.Count)
                .Select(item => item.Participant)
                .FirstAsync();

        public Task<ParticipantResultsDto[]> GetParticipantResults()
        {
            // Nur in dieser Form erlaubt EF das OrderByDescending
            // Auch das Verlagern des Sortieren auf den RAM macht keinen Performanceunterschied
            return DbSet
                .Select(p => new
                {
                    Country = p.Country.Name,
                    Points = p.Votes.Sum(v => v.Points),
                    Votes = p.Votes
                })
                .OrderByDescending(item => item.Points)
                .Select(item => new ParticipantResultsDto(
                    item.Country,
                    item.Points,
                    item.Votes
                        .OrderBy(r => r.Country.Name)
                        .Select(v => new ParticipantResult(v.Country.Name, v.Points))
                        .ToArray()
                ))
                .ToArrayAsync();
        }
    }
}
