using BookLibrary.Application.Abstractions.Messaging;
using BookLibrary.Domain.Libraries;

namespace BookLibrary.Application.Libraries.CreateLibrary;

public sealed record CreateLibraryCommand(LibraryName Name) : ICommand<Guid>;
