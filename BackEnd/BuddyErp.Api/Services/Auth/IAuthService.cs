using BuddyErp.Api.DTOs.Auth;

namespace BuddyErp.Api.Services.Auth;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default);
}
