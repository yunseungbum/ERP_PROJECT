namespace BuddyErp.Api.Services.Auth;

public sealed record AccessTokenResult(
    string AccessToken,
    DateTimeOffset ExpiresAt);
