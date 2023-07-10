using Entities.Models;

namespace Contracts;

public interface IUserRepository
{
    int CreateUser(User user);
    User FindUserByEmail(string email);
}