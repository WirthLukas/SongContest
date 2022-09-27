using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SongContest.Core.Entities
{
    public class ParticipantComposer : EntityObject
    {
        [Required] public string Name { get; set; }

        [ForeignKey(nameof(ParticipantId))] public Participant Participant { get; set; }
        public int ParticipantId { get; set; }
    }
}
