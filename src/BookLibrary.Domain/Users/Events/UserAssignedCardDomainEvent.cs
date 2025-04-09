using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.LibraryCards;

namespace BookLibrary.Domain.Users.Events;

public record UserAssignedCardDomainEvent(User User, LibraryCard Card) : IDomainEvent;