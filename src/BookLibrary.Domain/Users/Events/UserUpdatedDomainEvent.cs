using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.Users.Events;

public sealed record UserUpdatedDomainEvent(User User) : IDomainEvent;
