using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.Libraries;

public interface ILibraryRepository
{
    Task<Library?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    ValueTask<bool> ExistsAsync(LibraryName libraryName, CancellationToken cancellationToken);

    void Add(Library library);

    void Update(Library library);
}
