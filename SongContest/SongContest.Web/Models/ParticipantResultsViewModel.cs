using System.Collections.Generic;
using SongContest.Core.DataTransferObjects;

namespace SongContest.Web.Models
{
    public class ParticipantResultsViewModel
    {
        public IEnumerable<string> CountryNames { get; set; }
        public IEnumerable<ParticipantResultsDto> Participants { get; set; }

        public ParticipantResultsViewModel() {}
    }
}
