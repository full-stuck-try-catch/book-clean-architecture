using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.Users;

public sealed class Role : Entity
{
    public string Name { get; private set; }

    private Role(string name)
    {
        Name = name;
    }
}