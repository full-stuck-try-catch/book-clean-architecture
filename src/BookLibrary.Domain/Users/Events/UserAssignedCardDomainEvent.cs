using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.Users.Events;

public record UserAssignedCardDomainEvent(User User, Guid LibraryCardId) : IDomainEvent;