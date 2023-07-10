namespace Entities.Exceptions;

public class UserAlreadyExistsException : ConflictException
{
    public UserAlreadyExistsException(string email)
        : base($"User with email {email} already exists")
    {}
}