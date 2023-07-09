using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace AlinaverseAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public IActionResult RegisterUser(UserForRegistrationDto user)
    {
        var createdUser = _authService.RegisterUser(user);
        return Ok(new {User = createdUser, Token = _authService.CreateToken()});
    }

    [HttpPost("login")]
    public IActionResult Login(UserForAuthenticationServiceDto user)
    {
        if (!_authService.ValidateUser(user))
            return Unauthorized();
        return Ok(new {Token = _authService.CreateToken()});
    }
}