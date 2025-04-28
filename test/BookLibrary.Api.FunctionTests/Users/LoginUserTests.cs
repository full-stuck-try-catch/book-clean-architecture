using System.Net;
using System.Net.Http.Json;
using BookLibrary.Api.Controllers.Users;
using BookLibrary.Api.FunctionTests.Infrastructure;
using BookLibrary.Application.Users.LoginUser;
using FluentAssertions;

namespace BookLibrary.Api.FunctionTests.Users;

public class LoginUserTests : BaseFunctionalTest
{
    public LoginUserTests(FunctionalTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenUserDoesNotExist()
    {
        // Arrange
        var request = new LoginUserRequest("nonexistent@test.com", "wrongpassword");

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync("api/v1/users/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenUserExists()
    {
        // Arrange
        var request = new LoginUserRequest(
            UserData.RegisterTestUserRequest.email,
            UserData.RegisterTestUserRequest.password);

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync("api/v1/users/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        AccessTokenResponse? accessTokenResponse = await response.Content.ReadFromJsonAsync<AccessTokenResponse>();
        accessTokenResponse.Should().NotBeNull();
        accessTokenResponse!.AccessToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_ShouldReturnBadRequest_WhenPasswordIsIncorrect()
    {
        // Arrange
        var request = new LoginUserRequest(
            UserData.RegisterTestUserRequest.email,
            "wrongpassword");

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync("api/v1/users/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_ShouldReturnBadRequest_WhenEmailIsEmpty()
    {
        // Arrange
        var request = new LoginUserRequest("", "password123");

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync("api/v1/users/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_ShouldReturnBadRequest_WhenPasswordIsEmpty()
    {
        // Arrange
        var request = new LoginUserRequest("test@example.com", "");

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync("api/v1/users/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
