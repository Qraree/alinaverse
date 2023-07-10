using Shared.DataTransferObjects;

namespace Service.Contracts;

public interface IAuthService
{
    UserDto RegisterUser(UserForRegistrationDto userForRegistration);
    bool ValidateUser(UserForAuthenticationDto userForAuth);
    TokenDto CreateToken();
    TokenDto RefreshToken(TokenDto tokenDto);
    void LogOut(TokenDto tokenDto);
}