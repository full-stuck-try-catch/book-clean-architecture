using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.Users.Events;

public record UserUpdatedDomainEvent(User User) : IDomainEvent;