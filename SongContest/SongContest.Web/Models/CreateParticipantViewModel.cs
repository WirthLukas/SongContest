using System.ComponentModel.DataAnnotations;
using SongContest.Core.Entities;

namespace SongContest.Web.Models
{
    public class CreateParticipantViewModel
    {
        [Required] public string CountryName { get; set; }
        [Required] public string SongTitle { get; set; }
        [Required] public string ActorName { get; set; }

        public CreateParticipantViewModel() {}

        public Participant GetParticipant() => new()
        {
            SongTitle = SongTitle,
            ActorName = ActorName,
            Country = new Country { Name = CountryName }
        };
    }
}
