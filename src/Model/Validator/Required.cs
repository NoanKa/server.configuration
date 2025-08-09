using System.ComponentModel.DataAnnotations;

namespace Server.Model
{
    public class Required : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null)
            {
                throw new BusinessException(AppMessage.REQUIRED_PARAMETER);
            }

            return ValidationResult.Success;
        }
    }
}
