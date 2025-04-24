using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Books;

namespace BookLibrary.Domain.Libraries.Events;

public sealed record BookAddedToLibraryDomainEvent(Book Book) : IDomainEvent;
