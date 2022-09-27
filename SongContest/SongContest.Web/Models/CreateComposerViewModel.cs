using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SongContest.Core.Entities;

namespace SongContest.Web.Models
{
    public class CreateComposerViewModel
    {
        public int ParticipantId { get; set; }
        public string CountryName { get; set; }
        public string SongTitle { get; set; }
        [Required] public string NewComposerName { get; set; }

        public IEnumerable<string> ComposerNames { get; set; }

        public CreateComposerViewModel() {}

        public CreateComposerViewModel(Participant participant, IEnumerable<string> composerNames)
        {
            if (participant == null) throw new ArgumentNullException(nameof(participant));

            CountryName = participant.Country?.Name ??
                          throw new ArgumentNullException(nameof(participant), 
                              "Country of participant was null! (maybe not included in query?)");

            ParticipantId = participant.Id;
            SongTitle = participant.SongTitle;
            ComposerNames = composerNames;
        }

        public ParticipantComposer GetComposer() => new()
        {
            ParticipantId = ParticipantId,
            Name = NewComposerName
        };
    }
}
