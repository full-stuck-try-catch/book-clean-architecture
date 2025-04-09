using BookLibrary.Domain.Users;

namespace BookLibrary.Application.Abstractions.Authentication;

public interface IJwtService
{
    string GenerateToken(User user);
}
