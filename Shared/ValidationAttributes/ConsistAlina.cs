using System.ComponentModel.DataAnnotations;

namespace Shared.ValidationAttributes;

public class ContainsAlina : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        string str = (string) value;
        if (str != null && !str.Contains("alina", StringComparison.CurrentCultureIgnoreCase))
        {
            return new ValidationResult(ErrorMessage);
        }

        return ValidationResult.Success;
    }
}