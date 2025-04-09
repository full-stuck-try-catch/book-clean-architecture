namespace BookLibrary.Domain.Shared;

public record Email(string Value)
{
    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Email cannot be empty.");
        }

        if (!value.Contains("@"))
        {
            throw new ArgumentException("Email must contain @.");
        }

        return new Email(value);
    }
}
