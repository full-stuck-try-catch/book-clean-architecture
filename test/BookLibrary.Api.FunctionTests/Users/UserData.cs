using BookLibrary.Api.Controllers.Users;

namespace BookLibrary.Api.FunctionTests.Users;

internal static class UserData
{
    public static RegisterUserRequest RegisterTestUserRequest = new("test", "test", "test@test.com", "123456");
}
