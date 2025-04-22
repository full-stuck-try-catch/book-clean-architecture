using BookLibrary.Application.Abstractions.Authentication;
using BookLibrary.Application.Abstractions.Clock;
using BookLibrary.Application.Users.GetUserLogged;
using BookLibrary.Application.Users.LoginUser;
using BookLibrary.Application.Users.RegisterUser;
using BookLibrary.Application.Users.UpdateUserProfile;
using BookLibrary.Domain.Abstractions;
using BookLibrary.Domain.Shared;
using BookLibrary.Domain.Users;
using FluentAssertions;
using NSubstitute;

namespace BookLibrary.Application.UnitTests.Users;
public class UserTests
{
    private readonly GetLoggedUserQueryHandler _getLoggedUserQueryHandler;
    private readonly LoginUserCommandHandler _loginUserCommandHandler;
    private readonly RegisterUserCommandHandler _registerUserCommandHandler;
    private readonly UpdateUserProfileCommandHandler _updateUserProfileCommandHandler;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly IUserContext _userContext;
    private readonly IDateTimeProvider _dateTimeProvider;
    public UserTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _userContext = Substitute.For<IUserContext>();
        _passwordHasher = Substitute.For<IPasswordHasher>();
        _jwtService = Substitute.For<IJwtService>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();

        _getLoggedUserQueryHandler = new GetLoggedUserQueryHandler(_userRepository, _userContext);
        _loginUserCommandHandler = new LoginUserCommandHandler(_userRepository, _passwordHasher, _jwtService);
        _registerUserCommandHandler = new RegisterUserCommandHandler(_userRepository, _passwordHasher, _dateTimeProvider, Substitute.For<IUnitOfWork>());   
        _updateUserProfileCommandHandler = new UpdateUserProfileCommandHandler(_userRepository, _dateTimeProvider, Substitute.For<IUnitOfWork>());
    }

    [Fact]
    public async Task GetLoggedUser_ShouldReturnUserResponse_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = User.Create(
            userId,
            Email.Create("test@example.com"),
            new FirstName("John"),
            new LastName("Doe"),
            new PasswordHash("hashedPassword"),
            DateTime.UtcNow);

        _userContext.UserId.Returns(userId);
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);

        // Act
        Result<UserResponse> result = await _getLoggedUserQueryHandler.Handle(new GetLoggedUserQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Email.Should().Be("test@example.com");
        result.Value.FirstName.Should().Be("John");
        result.Value.LastName.Should().Be("Doe");
    }

    [Fact]
    public async Task LoginUser_ShouldReturnAccessToken_WhenCredentialsAreValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = User.Create(
            userId,
            Email.Create("test@example.com"),
            new FirstName("John"),
            new LastName("Doe"),
            new PasswordHash("hashedPassword"),
            DateTime.UtcNow);

        _userRepository.GetByEmailAsync("test@example.com", Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.Verify("hashedPassword", "password").Returns(true);
        _jwtService.GenerateToken(user).Returns("jwtToken");

        // Act
        Result<AccessTokenResponse> result = await _loginUserCommandHandler.Handle(new LoginUserCommand("test@example.com", "password"), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.AccessToken.Should().Be("jwtToken");
    }

    [Fact]
    public async Task RegisterUser_ShouldReturnUserId_WhenRegistrationIsSuccessful()
    {
        // Arrange
        _userRepository.IsEmailUniqueAsync("test@example.com", Arg.Any<CancellationToken>()).Returns(true);
        _passwordHasher.Hash("password").Returns("hashedPassword");
        _dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

        // Act
        Result<Guid> result = await _registerUserCommandHandler.Handle(new RegisterUserCommand("John", "Doe", "test@example.com", "password"), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task UpdateUserProfile_ShouldReturnSuccess_WhenUpdateIsSuccessful()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = User.Create(
            userId,
            Email.Create("test@example.com"),
            new FirstName("John"),
            new LastName("Doe"),
            new PasswordHash("hashedPassword"),
            DateTime.UtcNow);

        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);

        // Act
        Result result = await _updateUserProfileCommandHandler.Handle(new UpdateUserProfileCommand(userId, "Jane", "Smith"), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}
