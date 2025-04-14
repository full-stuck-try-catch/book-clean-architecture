namespace BookLibrary.Api.Controllers.Libraries;

public sealed record AddBookToLibraryRequest(
    string Title,
    string AuthorFirstName,
    string AuthorLastName,
    string AuthorCountry,
    int Quantity);
