using BookLibrary.Domain.Books;
using BookLibrary.Domain.Libraries;

namespace BookLibrary.Domain.UnitTests.Libraries;

internal static class LibraryData
{
    public static readonly LibraryName TestLibraryName = new("Test Library");

    public static Guid GetTestGuid() => Guid.NewGuid();

    public static Library CreateTestLibrary()
    {
        return Library.Create(GetTestGuid(), TestLibraryName);
    }

    public static Book CreateTestBook()
    {
        return Book.Create(
            Guid.NewGuid(),
            new BookTitle("Test Book Title"),
            new Author("Test", "Author", "USA"),
            1,
            Guid.NewGuid());
    }
}
