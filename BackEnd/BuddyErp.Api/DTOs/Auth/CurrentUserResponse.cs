namespace BuddyErp.Api.DTOs.Auth;

public sealed record CurrentUserResponse(
    long UserId,
    string Name,
    string Role);
