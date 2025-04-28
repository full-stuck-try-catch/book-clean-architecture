using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BookLibrary.Api.Controllers.Users;
using BookLibrary.Api.FunctionTests.Infrastructure;
using BookLibrary.Application.Users.GetUserLogged;
using BookLibrary.Application.Users.LoginUser;
using FluentAssertions;

namespace BookLibrary.Api.FunctionTests.Users;

public class GetLoggedUserTests : BaseFunctionalTest
{
    public GetLoggedUserTests(FunctionalTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetLoggedUser_ShouldReturnUnauthorized_WhenNoTokenProvided()
    {
        // Arrange
        // No authorization header set

        // Act
        HttpResponseMessage response = await HttpClient.GetAsync("api/v1/users/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetLoggedUser_ShouldReturnUnauthorized_WhenInvalidTokenProvided()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "invalid-token");

        // Act
        HttpResponseMessage response = await HttpClient.GetAsync("api/v1/users/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetLoggedUser_ShouldReturnOk_WhenValidTokenProvided()
    {
        // Arrange
        string accessToken = await GetAccessToken();
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        // Act
        HttpResponseMessage response = await HttpClient.GetAsync("api/v1/users/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        UserResponse? userResponse = await response.Content.ReadFromJsonAsync<UserResponse>();
        userResponse.Should().NotBeNull();
        userResponse!.Email.Should().Be(UserData.RegisterTestUserRequest.email);
        userResponse.FirstName.Should().Be(UserData.RegisterTestUserRequest.firstName);
        userResponse.LastName.Should().Be(UserData.RegisterTestUserRequest.lastName);
        userResponse.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetLoggedUser_ShouldReturnUnauthorized_WhenTokenIsExpired()
    {
        // Arrange
        string expiredToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyLCJleHAiOjE1MTYyMzkwMjN9.invalid";
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", expiredToken);

        // Act
        HttpResponseMessage response = await HttpClient.GetAsync("api/v1/users/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetLoggedUser_ShouldReturnUnauthorized_WhenTokenHasInvalidFormat()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "not.a.valid.jwt.token");

        // Act
        HttpResponseMessage response = await HttpClient.GetAsync("api/v1/users/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetLoggedUser_ShouldReturnUnauthorized_WhenAuthorizationHeaderIsMissing()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Clear();

        // Act
        HttpResponseMessage response = await HttpClient.GetAsync("api/v1/users/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetLoggedUser_ShouldReturnCorrectUserData_WhenMultipleUsersExist()
    {
        // Arrange
        // First, create a second user
        var secondUserRequest = new RegisterUserRequest("John", "Smith", "john.smith@test.com", "password123");
        await HttpClient.PostAsJsonAsync("api/v1/users/register", secondUserRequest);

        // Login as the second user
        var loginRequest = new LoginUserRequest(secondUserRequest.email, secondUserRequest.password);
        HttpResponseMessage loginResponse = await HttpClient.PostAsJsonAsync("api/v1/users/login", loginRequest);
        AccessTokenResponse? accessTokenResponse = await loginResponse.Content.ReadFromJsonAsync<AccessTokenResponse>();
        
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessTokenResponse!.AccessToken);

        // Act
        HttpResponseMessage response = await HttpClient.GetAsync("api/v1/users/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        UserResponse? userResponse = await response.Content.ReadFromJsonAsync<UserResponse>();
        userResponse.Should().NotBeNull();
        userResponse!.Email.Should().Be(secondUserRequest.email);
        userResponse.FirstName.Should().Be(secondUserRequest.firstName);
        userResponse.LastName.Should().Be(secondUserRequest.lastName);
        userResponse.Id.Should().NotBeEmpty();
    }
}
