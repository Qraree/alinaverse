using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects;

public record UserForAuthenticationDto(
    [EmailAddress] string Email, 
    [MinLength(8)] string Password);