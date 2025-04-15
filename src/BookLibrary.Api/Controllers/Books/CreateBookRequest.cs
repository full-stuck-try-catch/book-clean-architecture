namespace BookLibrary.Api.Controllers.Books;

public sealed record CreateBookRequest(
    string Title,
    string AuthorFirstName,
    string AuthorLastName,
    string AuthorCountry,
    int Quantity,
    Guid LibraryId);
