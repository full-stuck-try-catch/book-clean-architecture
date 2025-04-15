using BookLibrary.Application.Abstractions.Messaging;

namespace BookLibrary.Application.Books.MarkBookAsReturned;

public sealed record MarkBookAsReturnedCommand(Guid BookId) : ICommand;
