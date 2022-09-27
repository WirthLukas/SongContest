using System;
using System.ComponentModel.DataAnnotations;

namespace SongContest.Core.Validations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CorrectPointValueAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            int points = (int) value;

            var validationResult = points switch
            {
                (>= 0 and <= 8) or 10 or 12 => ValidationResult.Success,
                _ => new ValidationResult($"Point Value must be 0,1,2,3,4,5,6,7,8,10 or 12 but is {points}",
                    new[] {validationContext.MemberName})
            };

            return validationResult;
        }
    }
}
