using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SongContest.Core.Entities
{
    public class Participant : EntityObject
    {
        public int StartNumber { get; set; }
        [Required] public string SongTitle { get; set; }
        [Required] public string ActorName { get; set; }

        [ForeignKey(nameof(CountryId))] public Country Country { get; set; }
        public int CountryId { get; set; }

        public ICollection<ParticipantComposer> Composers { get; set; } = new List<ParticipantComposer>();
        public ICollection<Vote> Votes { get; set; } = new List<Vote>();
    }
}
