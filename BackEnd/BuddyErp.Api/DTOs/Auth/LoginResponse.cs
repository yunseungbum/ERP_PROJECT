namespace BuddyErp.Api.DTOs.Auth;

public sealed record LoginResponse(
    long UserId,
    string Name,
    IReadOnlyList<string> Roles,
    string AccessToken,
    DateTimeOffset ExpiresAt);
