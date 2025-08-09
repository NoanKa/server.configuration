using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Server.Model
{
    public class Email : ValidationAttribute
    {
        private string pattern = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }
            else if (!Regex.IsMatch(value.ToString(), pattern))
            {
                throw new BusinessException(AppMessage.AUTH_EMAIL_INVALID);
            }

            return ValidationResult.Success;
        }
    }
}
