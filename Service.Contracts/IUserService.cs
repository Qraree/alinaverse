using Shared.DataTransferObjects;

namespace Service.Contracts;

public interface IUserService
{
    UserDto AddUser(UserForCreationDto user);
}