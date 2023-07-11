using Microsoft.AspNetCore.Authorization;
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
        var tokenDtoResult = _authService.CreateToken();
        
        HttpContext.Response.Cookies.Append("refreshToken", tokenDtoResult.RefreshToken,
            new CookieOptions()
            {
                HttpOnly = true,
                Expires = DateTime.Now.AddDays(30),
                SameSite = SameSiteMode.None,
                Secure = true
            });
        
        return Ok(new {User = createdUser, AccessToken = tokenDtoResult.AccessToken});
    }

    [HttpPost("login")]
    public IActionResult Login(UserForAuthenticationDto user)
    {
        if (!_authService.ValidateUser(user))
            return Unauthorized();
        var tokenDtoResult = _authService.CreateToken();
        
        HttpContext.Response.Cookies.Append("refreshToken", tokenDtoResult.RefreshToken,
            new CookieOptions()
            {
                HttpOnly = true,
                Expires = DateTime.Now.AddDays(30),
                SameSite = SameSiteMode.None,
                Secure = true
            });
        
        return Ok(new {AccessToken = tokenDtoResult.AccessToken});
    }

    [HttpGet("refresh")]
    public IActionResult Refresh()
    {
        var token = Request.Headers["Authorization"].ToString().Substring("Bearer ".Length);;
        var refreshToken = HttpContext.Request.Cookies["refreshToken"];
        
        var tokenDtoResult = _authService.RefreshToken(new TokenDto(token, refreshToken));
        
        HttpContext.Response.Cookies.Append("refreshToken", tokenDtoResult.RefreshToken,
            new CookieOptions()
            {
                HttpOnly = true,
                Expires = DateTime.Now.AddDays(30),
                SameSite = SameSiteMode.None,
                Secure = true
            });
        
        return Ok(new {AccessToken = tokenDtoResult.AccessToken});
    }

    [Authorize]
    [HttpPost("logout")]
    public IActionResult LogOut(string accessToken)
    {
        var token = HttpContext.Request.Cookies["refreshToken"];
        _authService.LogOut(new TokenDto(accessToken, token));

        HttpContext.Response.Cookies.Delete("refreshToken");
        
        return Ok();
    }
    
}