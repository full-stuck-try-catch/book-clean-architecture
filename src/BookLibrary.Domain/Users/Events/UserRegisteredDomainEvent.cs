using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.Users.Events;

public record UserRegisteredDomainEvent(User User) : IDomainEvent;