namespace Entities.Exceptions;

public class UserNotFoundException : NotFoundException
{
    public UserNotFoundException(string email) 
        : base($"User with email {email} not found")
    {}
}