using BookLibrary.Application.Abstractions.Clock;

namespace BookLibrary.Infrastructure.Clock;

internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
