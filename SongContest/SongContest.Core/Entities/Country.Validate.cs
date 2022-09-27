using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using SongContest.Core.Contracts;

namespace SongContest.Core.Entities
{
    public partial class Country : IDatabaseValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (validationContext.GetService(typeof(IUnitOfWork)) is not IUnitOfWork unitOfWork)
            {
                yield break;
                //throw new AccessViolationException("UnitOfWork is not injected!");
            }

            var count = Task.Run(() => unitOfWork.Countries.CountAsync(c => c.Name == this.Name))
                .Result;

            if (count is not 0)
            {
                yield return new ValidationResult($"There is already a Country with name {Name}",
                    new[] {nameof(Name)});
            }
        }
    }
}
