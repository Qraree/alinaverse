using System.Data;
using Contracts;
using Dapper;
using Entities.Models;
using Microsoft.Extensions.Configuration;

namespace Repository;

public class TokenRepository : RepositoryBase, ITokenRepository
{
    public TokenRepository(IConfiguration configuration)
        : base(configuration)
    { }

    public Token GetRefreshToken(int userId, string token)
    {
        using (IDbConnection dbConnection = Connection)
        {
            dbConnection.Open();
            return dbConnection.Query<Token>("SELECT * FROM refresh_tokens WHERE user_id = @Id " +
                                             "AND refresh_token = @Token", new {Id = userId, Token = token})
                .FirstOrDefault();
        }
    }

    public void SetUpRefreshToken(Token token)
    {
        using (IDbConnection dbConnection = Connection)
        {
            dbConnection.Open();
            dbConnection.Execute("INSERT INTO refresh_tokens (user_id, refresh_token, expiry_time) " +
                                 "VALUES (@UserId, @RefreshToken, @ExpiryTime)", token);
        }
    }

    public void DeleteRefreshToken(int id)
    {
        using (IDbConnection dbConnection = Connection)
        {
            dbConnection.Open();
            dbConnection.Execute("DELETE FROM refresh_tokens WHERE id = @Id", new {Id = id});
        }
    }
}