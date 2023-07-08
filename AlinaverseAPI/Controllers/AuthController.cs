using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace AlinaverseAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAuthService _authService;

    public AuthController(IUserService userService, IAuthService authService)
    {
        _userService = userService;
        _authService = authService;
    }

    [HttpPost("register")]
    public IActionResult RegisterUser(UserForCreationDto user)
    {
        var createdUser = _userService.AddUser(user);
        return Ok(new { User = createdUser, Token = _authService.CreateToken()});
    }
}