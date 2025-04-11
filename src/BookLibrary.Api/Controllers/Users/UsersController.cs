using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookLibrary.Api.Controllers.Users;

[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/users")]
public class UsersController : ControllerBase
{
    private readonly ISender _sender;

    public UsersController(ISender sender)
    {
        _sender = sender;
    }
}
