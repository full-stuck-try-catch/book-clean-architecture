using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookLibrary.Api.Controllers.Users;

namespace BookLibrary.Application.IntegrationTests.Infrastructure;
internal static class TestData
{
    public static Guid TestUserId = Guid.NewGuid();
    public static RegisterUserRequest RegisterTestUserRequest = new("test", "test", "test@test.com", "123456");
}
