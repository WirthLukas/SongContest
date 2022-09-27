using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using SongContest.Core.Contracts;

namespace SongContest.Core.Entities
{
    public partial class Vote : IDatabaseValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (validationContext.GetService(typeof(IUnitOfWork)) is not IUnitOfWork unitOfWork)
            {
                yield break;
                //throw new AccessViolationException("UnitOfWork is not injected!");
            }

            (bool samePoints, bool sameCountry) = Task.Run(() => unitOfWork.Votes
                    .IsVoteOfCountryAllowed(this.CountryId, this.Points, this.Participant.CountryId))
                .Result;

            if (samePoints)
            {
                yield return new ValidationResult($"{Points} Punkte wurden bereits vergeben!",
                    new[] { nameof(Points) });
            }

            if (sameCountry)
            {
                yield return new ValidationResult($"Es wurde bereits für {this.Participant.Country?.Name ?? "null"} abgestimmt!",
                    new[] { nameof(Participant) });
            }
        }
    }
}
