using System;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BookLibrary.Api.Controllers.Books;
using BookLibrary.Api.Controllers.Libraries;
using BookLibrary.Api.Controllers.Users;
using BookLibrary.Api.FunctionTests.Infrastructure;
using FluentAssertions;

namespace BookLibrary.Api.FunctionTests.Users;

public class UserBorrowBookTests : BaseFunctionalTest
{
    public UserBorrowBookTests(FunctionalTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task BorrowBook_ShouldReturnUnauthorized_WhenNoTokenProvided()
    {
        // Arrange
        var request = new BorrowBookUserRequest(
            Guid.NewGuid(),
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(14));

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync("api/v1/users/borrow-book", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task BorrowBook_ShouldReturnUnauthorized_WhenInvalidTokenProvided()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "invalid-token");
        var request = new BorrowBookUserRequest(
            Guid.NewGuid(),
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(14));

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync("api/v1/users/borrow-book", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task BorrowBook_ShouldReturnBadRequest_WhenBookNotFound()
    {
        // Arrange
        string accessToken = await GetAccessToken();
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var request = new BorrowBookUserRequest(
            Guid.NewGuid(), // Non-existent book ID
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(14));

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync("api/v1/users/borrow-book", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task BorrowBook_ShouldReturnBadRequest_WhenStartDateIsInPast()
    {
        // Arrange
        string accessToken = await GetAccessToken();
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var createLibraryRequest = new CreateLibraryRequest("Test Library for Borrowing 1");

        Guid libraryId = await CreateTestLibrary(accessToken, createLibraryRequest);
        Guid bookId = await CreateTestBook(libraryId, accessToken, "Book 1");

        var borrowRequest = new BorrowBookUserRequest(
            bookId,
            DateTime.UtcNow.AddDays(-5), // Past date
            DateTime.UtcNow.AddDays(14));

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync("api/v1/users/borrow-book", borrowRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task BorrowBook_ShouldReturnBadRequest_WhenEndDateIsBeforeStartDate()
    {
        // Arrange
        string accessToken = await GetAccessToken();
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var createLibraryRequest = new CreateLibraryRequest("Test Library for Borrowing 2");
        Guid libraryId = await CreateTestLibrary(accessToken, createLibraryRequest);
        Guid bookId = await CreateTestBook(libraryId, accessToken, "Book 2");

        var borrowRequest = new BorrowBookUserRequest(
            bookId,
            DateTime.UtcNow.AddDays(14),
            DateTime.UtcNow.AddDays(7)); // End date before start date

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync("api/v1/users/borrow-book", borrowRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task BorrowBook_ShouldReturnBadRequest_WhenBookIdIsEmpty()
    {
        // Arrange
        string accessToken = await GetAccessToken();
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var borrowRequest = new BorrowBookUserRequest(
            Guid.Empty,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(14));

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync("api/v1/users/borrow-book", borrowRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task BorrowBook_ShouldReturnBadRequest_WhenUserAlreadyBorrowedBook()
    {
        // Arrange
        string accessToken = await GetAccessToken();
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var createLibraryRequest = new CreateLibraryRequest("Test Library for Borrowing 3");
        Guid libraryId = await CreateTestLibrary(accessToken, createLibraryRequest);
        Guid bookId = await CreateTestBook(libraryId, accessToken, "Book3");

        var borrowRequest = new BorrowBookUserRequest(
            bookId,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(14));

        // First borrow attempt - should succeed
        await HttpClient.PostAsJsonAsync("api/v1/users/borrow-book", borrowRequest);

        // Act - Second borrow attempt - should fail
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync("api/v1/users/borrow-book", borrowRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private async Task<Guid> CreateTestLibrary(string accessToken , CreateLibraryRequest createLibraryRequest)
    {
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        
        HttpResponseMessage libraryResponse = await HttpClient.PostAsJsonAsync("api/v1/libraries", createLibraryRequest);
        
        libraryResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        Guid libraryId = await libraryResponse.Content.ReadFromJsonAsync<Guid>();
        
        return libraryId;
    }

    private async Task<Guid> CreateTestBook(Guid libraryId, string accessToken, string title = "Test Book for Borrowing")
    {
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        
        var createBookRequest = new CreateBookRequest(
            title,
            "John",
            "Doe",
            "USA",
            5,
            libraryId);
        
        HttpResponseMessage bookResponse = await HttpClient.PostAsJsonAsync("api/v1/books", createBookRequest);
        
        bookResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        Guid bookId = await bookResponse.Content.ReadFromJsonAsync<Guid>();
        
        return bookId;
    }
}
