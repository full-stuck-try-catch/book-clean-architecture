using BookLibrary.Application.Abstractions.Messaging;

namespace BookLibrary.Application.Books.AddStock;

public sealed record AddStockCommand(
    Guid BookId,
    int Count) : ICommand;
