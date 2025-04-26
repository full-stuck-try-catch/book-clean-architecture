using System.Configuration;
using System.Security.Claims;
using System.Text;
using BookLibrary.Application.Abstractions.Authentication;
using BookLibrary.Domain.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace BookLibrary.Infrastructure.Authentication;

internal sealed class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;
    public JwtService(IConfiguration configuration) { _configuration = configuration; }

    public string GenerateToken(User user)
    {
        JwtAuthenticationOptions jwtSettings = _configuration.GetSection("JwtAuthentication").Get<JwtAuthenticationOptions>() ??
                                                      throw new InvalidOperationException("JwtAuthentication configuration section is missing.");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email.Value)
            };

        // Add role claims
        claims.AddRange(user.Roles.Select(r => new Claim(ClaimTypes.Role, r.Name)));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(jwtSettings.ExpiresInMinutes),
            SigningCredentials = credentials,
            Issuer = jwtSettings.Issuer,
            Audience = jwtSettings.Audience
        };

        var handler = new JsonWebTokenHandler();

        string token = handler.CreateToken(tokenDescriptor);

        return token;
    }
}
