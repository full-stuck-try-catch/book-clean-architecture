using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.Users.Events;

public sealed record UserAssignedCardDomainEvent(User User, Guid LibraryCardId) : IDomainEvent;
