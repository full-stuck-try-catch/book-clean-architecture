using System.Net;
using System.Net.Http.Json;
using BookLibrary.Api.FunctionTests.Infrastructure;
using FluentAssertions;

namespace BookLibrary.Api.FunctionTests.Users;

public class LoginUserTests : BaseFunctionalTest
{
    private const string Email = "login@test.com";
    private const string Password = "12345";

    public LoginUserTests(FunctionalTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenUserDoesNotExist()
    {
        // Arrange

        // Act

        // Assert
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenUserExists()
    {
        // Arrange
       
        // Act

        // Assert
    }
}
