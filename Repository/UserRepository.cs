using System.Data;
using System.Linq.Expressions;
using Contracts;
using Dapper;
using Entities.Models;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Repository;

public class UserRepository : RepositoryBase, IUserRepository
{
    public UserRepository(IConfiguration configuration)
        : base(configuration) {}

    public IQueryable<User> FindAll(Expression<Func<User, bool>> expression)
    {
        throw new NotImplementedException();
    }

    public User FindById(int id)
    {
        using (IDbConnection dbConnection = Connection)
        {
            dbConnection.Open();
            return dbConnection.Query<User>(
                "SELECT * FROM users WHERE id = @Id", new {Id = id}).FirstOrDefault();
        }
    }

    public int CreateUser(User entity)
    {
        using (IDbConnection dbConnection = Connection)
        {
            dbConnection.Open();
            return dbConnection.ExecuteScalar<int>(
                "INSERT INTO users (name, password, email) VALUES (@Name, @Password, @Email) RETURNING id", entity);
        }
    }

    public User FindUserByEmail(string email)
    {
        using (IDbConnection dbConnection = Connection)
        {
            dbConnection.Open();
            return dbConnection.Query<User>("SELECT * FROM users WHERE email = @Email", new {Email = email})
                .FirstOrDefault();
        }
    }
}