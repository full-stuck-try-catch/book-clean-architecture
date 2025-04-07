using BookLibrary.Domain.Abstractions;

namespace BookLibrary.Domain.Libraries;

public interface ILibraryRepository
{
    Task<Library> GetByIdAsync(Guid id);

    ValueTask<bool> ExistsAsync(string libraryName);

    void Add(Library library);

    void Delete(Library library);
}