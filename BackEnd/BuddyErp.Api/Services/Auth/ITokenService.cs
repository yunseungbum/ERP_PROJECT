using BuddyErp.Api.Data.Entities;

namespace BuddyErp.Api.Services.Auth;

public interface ITokenService
{
    AccessTokenResult CreateAccessToken(User user);
}
