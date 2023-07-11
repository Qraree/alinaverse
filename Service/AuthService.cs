using Service.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Contracts;
using Entities.ConfigurationModels;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.DataTransferObjects;

namespace Service;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenRepository _tokenRepository;
    private readonly JwtConfiguration _jwtConfiguration;
    private User _user;

    public AuthService(IUserRepository userRepository, 
        ITokenRepository tokenRepository, IOptions<JwtConfiguration> configuration)
    {
        _userRepository = userRepository;
        _tokenRepository = tokenRepository;
        _jwtConfiguration = configuration.Value;
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

    public bool ValidateUser(UserForAuthenticationDto userForAuth)
    {
        _user = _userRepository.FindUserByEmail(userForAuth.Email);
        if (this._user is null)
            throw new UserNotFoundException(userForAuth.Email);
        return VerifyPassword(userForAuth.Password, _user.Password);
    }

    public TokenDto CreateToken()
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.SecretKey));
        var signInCred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        var claims = GetClaims();
        var tokenOptions = GenerateTokenOptions(signInCred, claims);

        var refreshToken = GenerateRefreshToken();

        var newToken = new Token()
        {
            UserId = _user.Id,
            RefreshToken = refreshToken,
            ExpiryTime = DateTime.Now.AddDays(_jwtConfiguration.RefreshTokenLifeSpan)
        };
        
        _tokenRepository.SetUpRefreshToken(newToken);
        
        var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        return new TokenDto(accessToken, refreshToken);
    }

    public TokenDto RefreshToken(TokenDto tokenDto)
    {
        var principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken);

        var user = _userRepository.FindUserByEmail(principal.FindFirst(ClaimTypes.Email).Value);
        if (user == null)
            throw new RefreshTokenBadRequest();

        var refreshToken = _tokenRepository.GetRefreshToken(user.Id, tokenDto.RefreshToken);
        if (refreshToken == null || refreshToken.ExpiryTime <= DateTime.Now)
            throw new RefreshTokenBadRequest();
        
        _tokenRepository.DeleteRefreshToken(refreshToken.Id);
        
        _user = user;
        
        return CreateToken();
    }

    public void LogOut(TokenDto tokenDto)
    {
        var principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken);

        var user = _userRepository.FindUserByEmail(principal.FindFirst(ClaimTypes.Email).Value);
        if (user == null)
            throw new RefreshTokenBadRequest();

        var refreshToken = _tokenRepository.GetRefreshToken(user.Id, tokenDto.RefreshToken);
        if (refreshToken == null || refreshToken.ExpiryTime <= DateTime.Now)
            throw new RefreshTokenBadRequest();
        
        _tokenRepository.DeleteRefreshToken(refreshToken.Id);
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtConfiguration.ValidIssuer,
            ValidAudience = _jwtConfiguration.ValidAudience,
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.SecretKey))
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken == null ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature,
                StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }

    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
    {
        var tokenOptions = new JwtSecurityToken
        (
            issuer: _jwtConfiguration.ValidIssuer,
            audience: _jwtConfiguration.ValidAudience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_jwtConfiguration.LifeSpan),
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