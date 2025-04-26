using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.Users;

public sealed class Role
{
    public static readonly Role Admin = new(1, "Admin");
    public static readonly Role User = new(2, "User");

    public int Id { get; init; }
    public string Name { get; init; }

    public ICollection<User> Users { get; init; }

    public ICollection<Permission> Permissions { get; init; } = new List<Permission>();

    private Role(int id, string name)
    {
        Id = id;
        Name = name;
    }
}
