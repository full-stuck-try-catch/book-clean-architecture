using BookLibrary.Application.Abstractions.Messaging;

namespace BookLibrary.Application.Books.MarkBookAsDeleted;

public sealed record MarkBookAsDeletedCommand(Guid BookId) : ICommand;
