using BookLibrary.Application.Abstractions.Messaging;

namespace BookLibrary.Application.Libraries.CreateLibrary;

public sealed record CreateLibraryCommand(string Name) : ICommand<Guid>;
