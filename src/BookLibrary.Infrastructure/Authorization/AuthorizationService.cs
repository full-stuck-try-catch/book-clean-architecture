using BookLibrary.Application.Abstractions.Caching;
using BookLibrary.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Infrastructure.Authorization;

internal sealed class AuthorizationService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICacheService _cacheService;

    public AuthorizationService(ApplicationDbContext dbContext, ICacheService cacheService)
    {
        _dbContext = dbContext;
        _cacheService = cacheService;
    }

    public async Task<List<string>> GetRolesForUserAsync(string identityId)
    {
        string cacheKey = $"auth:roles-{identityId}";

        if (!Guid.TryParse(identityId, out Guid userId))
        {
            throw new InvalidOperationException("Invalid identity ID.");
        }


        List<string> cachedRoles = await _cacheService.GetAsync<List<string>>(cacheKey);

        if (cachedRoles is not null)
        {
            return cachedRoles;
        }

        List<string> roles = await _dbContext.Set<User>()
            .Where(u => u.Id == userId)
            .SelectMany(u => u.Roles.Select(r => r.Name))
            .ToListAsync();
        await _cacheService.SetAsync(cacheKey, roles);

        return roles;
    }
}
