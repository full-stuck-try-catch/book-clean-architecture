namespace BookLibrary.Domain.Users;
public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);

    Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken);

    void Add(User user);

    void Update(User user);
}
