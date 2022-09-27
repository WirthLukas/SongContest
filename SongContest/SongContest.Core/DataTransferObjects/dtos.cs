using System.Collections.Generic;

namespace SongContest.Core.DataTransferObjects
{
    public record CountryStatisticsDto(int Id, string Country, int GivenVotes, int? Points);

    public record CountryVotesDto(int VoteId, string VotedCountry, int Points);

    public record ParticipantResult(string Country, int Points);

    public record ParticipantResultsDto(string Country, int Points, IEnumerable<ParticipantResult> Results);
}
