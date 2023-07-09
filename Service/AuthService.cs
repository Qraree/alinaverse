using Service.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.IdentityModel.Tokens;
using Shared.DataTransferObjects;

namespace Service;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenRepository _tokenRepository;
    private User _user;

    public AuthService(IUserRepository userRepository, ITokenRepository tokenRepository)
    {
        _userRepository = userRepository;
        _tokenRepository = tokenRepository;
    }

    private string HashPassword(string password)
    {
        byte[] salt;
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt = new byte[16]);
        }

        var pbkf2 = new Rfc2898DeriveBytes(password, salt, 1000);
        byte[] hash = pbkf2.GetBytes(20);

        byte[] hashBytes = new byte[36];
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 20);
        
        return Convert.ToBase64String(hashBytes);
    }
    
    private bool VerifyPassword(string currentPassword, string hashedPassword)
    {
        byte[] hashBytes = Convert.FromBase64String(hashedPassword);

        byte[] salt = new byte[16];
        Array.Copy(hashBytes, 0, salt, 0, 16);
        var pbkf2 = new Rfc2898DeriveBytes(currentPassword, salt, 1000);
        byte[] hash = pbkf2.GetBytes(20);
        for (int i = 0; i < 20; i++)
            if (hashBytes[i + 16] != hash[i])
                return false;
        return true;
    }

    public UserDto RegisterUser(UserForRegistrationDto userForRegistration)
    {
        var user = _userRepository.FindUserByEmail(userForRegistration.Email);
        if (user is not null)
            throw new UserAlreadyExistsException(userForRegistration.Email);
        var hashedPass = HashPassword(userForRegistration.Password);
        var userId = _userRepository.CreateUser(new User()
        {
            Email = userForRegistration.Email,
            Name = userForRegistration.Name,
            Password = hashedPass
        });
        _user = _userRepository.FindUserByEmail(userForRegistration.Email);
        return new UserDto(userId, userForRegistration.Name, userForRegistration.Email);
    }

    public bool ValidateUser(UserForAuthenticationServiceDto userForAuth)
    {
        _user = _userRepository.FindUserByEmail(userForAuth.Email);
        if (this._user is null)
            throw new UserNotFoundException(userForAuth.Email);
        return VerifyPassword(userForAuth.Password, _user.Password);
    }

    public TokenDto CreateToken()
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("3ZcRUst4DM9M24kree5uupQyLmL6pRARw7xxuPAtH"));
        var signInCred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        var claims = GetClaims();
        var tokenOptions = GenerateTokenOptions(signInCred, claims);

        var refreshToken = GenerateRefreshToken();
        _tokenRepository.SetUpRefreshToken(_user.Id, refreshToken, DateTime.Now.AddDays(7));
        
        var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        return new TokenDto(accessToken, refreshToken);
    }

    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
    {
        var tokenOptions = new JwtSecurityToken
        (
            issuer: "Alinaverse",
            audience: "https://localhost:7023",
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: signingCredentials
        );
        return tokenOptions;
    }

    private List<Claim> GetClaims()
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, _user.Email)
        };
        return claims;
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}