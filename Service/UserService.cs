using Contracts;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    public UserDto AddUser(UserForCreationDto user)
    {
        var newUser = new User()
        {
            Name = user.Name,
            Password = user.Password
        };
        var userId = _repository.CreateUser(newUser);
        return new UserDto(userId, newUser.Name);
    }
}