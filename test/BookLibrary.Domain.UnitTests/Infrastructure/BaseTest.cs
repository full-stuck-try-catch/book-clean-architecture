using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.UnitTests.Infrastructure;
public abstract class BaseTest
{
    public static T AssertDomainEventWasPublished<T>(Entity entity)
       where T : IDomainEvent
    {
        T? domainEvent = entity.GetDomainEvents().OfType<T>().SingleOrDefault();

        if (domainEvent is not null)
        {
            return domainEvent;
        }

        throw new Exception($"{typeof(T).Name} was not published");
    }
}
