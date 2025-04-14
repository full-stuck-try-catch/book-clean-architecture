using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.Users.Events;
public sealed record UserRegisteredDomainEvent(User User) : IDomainEvent;
