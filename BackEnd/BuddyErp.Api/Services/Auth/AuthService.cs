using BuddyErp.Api.Data;
using BuddyErp.Api.Data.Entities;
using BuddyErp.Api.DTOs.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BuddyErp.Api.Services.Auth;

public sealed class AuthService(
    AppDbContext dbContext,
    IPasswordHasher<User> passwordHasher,
    ITokenService tokenService) : IAuthService
{
    public async Task<LoginResponse?> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var normalizedLoginId = request.LoginId.Trim().ToLowerInvariant();
        var user = await dbContext.Users.SingleOrDefaultAsync(
            user =>
                user.LoginId == normalizedLoginId && user.IsActive,
            cancellationToken);

        if (user is null)
        {
            return null;
        }

        var verificationResult = passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.Password);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return null;
        }

        if (verificationResult ==
            PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = passwordHasher.HashPassword(
                user,
                request.Password);
            user.UpdatedAt = DateTime.UtcNow;
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var accessToken = tokenService.CreateAccessToken(user);

        return new LoginResponse(
            user.UserId,
            user.DisplayName,
            [user.RoleCode],
            accessToken.AccessToken,
            accessToken.ExpiresAt);
    }
}
