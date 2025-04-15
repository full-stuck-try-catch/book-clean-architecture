namespace BookLibrary.Application.Books.GetBook;

public sealed record BookResponse(
    Guid Id,
    string Title,
    string AuthorFirstName,
    string AuthorLastName,
    string AuthorCountry,
    Guid LibraryId,
    int Quantity,
    int AvailableQuantity,
    string Status,
    bool IsAvailable);
