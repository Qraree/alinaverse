using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects;

public record UserForAuthenticationServiceDto(
    [EmailAddress] string Email, 
    [MinLength(8)] string Password);