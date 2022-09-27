using System.ComponentModel.DataAnnotations.Schema;
using SongContest.Core.Validations;

namespace SongContest.Core.Entities
{
    public partial class Vote : EntityObject
    {
        [CorrectPointValue] public int Points { get; set; }

        [ForeignKey(nameof(CountryId))] public Country Country { get; set; }
        public int CountryId { get; set; }

        [ForeignKey(nameof(ParticipantId))] public Participant Participant { get; set; }
        public int ParticipantId { get; set; }
    }
}
