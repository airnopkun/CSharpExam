using System;
using System.ComponentModel.DataAnnotations;

namespace Exam.Validations
{
    public class FutureAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime date;
            if (DateTime.TryParse(value.ToString(), out date))
            {
                if(date < DateTime.Now)
                {
                    return new ValidationResult("Event must be in the future");
                    
                }
                return ValidationResult.Success;
            }
            return new ValidationResult("Uh oh");
        }
    }
}