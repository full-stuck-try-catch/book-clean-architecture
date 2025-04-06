using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Books;

namespace BookLibrary.Domain.Libraries.Events;

public record BookRemovedFromLibraryDomainEvent(Book Book) : IDomainEvent;