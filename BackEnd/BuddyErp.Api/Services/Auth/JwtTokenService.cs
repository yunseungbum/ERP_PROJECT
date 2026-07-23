using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BuddyErp.Api.Data.Entities;
using BuddyErp.Api.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BuddyErp.Api.Services.Auth;

public sealed class JwtTokenService(
    IOptions<JwtOptions> jwtOptions) : ITokenService
{
    private readonly JwtOptions options = jwtOptions.Value;

    public AccessTokenResult CreateAccessToken(User user)
    {
        var issuedAt = DateTimeOffset.UtcNow;
        var expiresAt = issuedAt.AddMinutes(
            options.AccessTokenMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new(JwtRegisteredClaimNames.Name, user.DisplayName),
            new("role", user.RoleCode),
            new(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString()),
        };

        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(options.SigningKey));
        var signingCredentials = new SigningCredentials(
            signingKey,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: options.Audience,
            claims: claims,
            notBefore: issuedAt.UtcDateTime,
            expires: expiresAt.UtcDateTime,
            signingCredentials: signingCredentials);

        return new AccessTokenResult(
            new JwtSecurityTokenHandler().WriteToken(token),
            expiresAt);
    }
}
