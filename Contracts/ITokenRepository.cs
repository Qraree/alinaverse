using Entities.Models;

namespace Contracts;

public interface ITokenRepository
{
    Token GetRefreshToken(int userId, string token);
    void SetUpRefreshToken(Token token);
    void DeleteRefreshToken(int id);
}