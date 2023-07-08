using Service.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Service;

public class AuthService : IAuthService
{
    public string CreateToken()
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("3ZcRUst4DM9M24kree5uupQyLmL6pRARw7xxuPAtH"));
        var signInCred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        var tokenOptions = new JwtSecurityToken
        (
            issuer: "Alinaverse",
            audience: "https://localhost:7023",
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: signInCred
        );
        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }
}