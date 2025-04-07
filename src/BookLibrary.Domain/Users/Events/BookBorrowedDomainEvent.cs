using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Books;

namespace BookLibrary.Domain.Users.Events;

public record BookBorrowedDomainEvent(Book Book) : IDomainEvent;