using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.Users;

public sealed class Role
{
    public static readonly Role User = new(1, "User");
    public static readonly Role Admin = new(2, "Admin");

    public int Id { get; private set; }
    public string Name { get; private set; }
    public ICollection<User> Users { get; private set; }

    private Role(int id, string name)
    {
        Id = id;
        Name = name;
    }
}
