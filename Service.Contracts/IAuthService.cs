using Shared.DataTransferObjects;

namespace Service.Contracts;

public interface IAuthService
{
    UserDto RegisterUser(UserForRegistrationDto userForRegistration);
    bool ValidateUser(UserForAuthenticationServiceDto userForAuth);
    TokenDto CreateToken();
}