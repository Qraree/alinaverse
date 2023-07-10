using System.ComponentModel.DataAnnotations;
using Shared.ValidationAttributes;

namespace Shared.DataTransferObjects;

public record UserForRegistrationDto(
    [Required, ContainsAlina(ErrorMessage = "The username must contain Alina")] string Name, 
    [EmailAddress] string Email, 
    [Required, MinLength(8)] string Password);