namespace BookLibrary.Domain.Users;
public sealed class Permission
{
    public static readonly Permission Admin = new(1, "admins:manage");
    public static readonly Permission UserRead = new(2, "users:read");

    private Permission(int id , string name) { 
        Id = id;
        Name = name;
    }
    public int Id { get; init; }

    public string Name { get; init; }
}
