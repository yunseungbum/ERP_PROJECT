using BuddyErp.Api.DTOs.Auth;
using BuddyErp.Api.Services.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuddyErp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var response = await authService.LoginAsync(
            request,
            cancellationToken);

        if (response is null)
        {
            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "로그인 실패",
                detail: "아이디 또는 비밀번호가 올바르지 않습니다.");
        }

        return Ok(response);
    }

    [Authorize]
    [HttpGet("me")]
    public ActionResult<CurrentUserResponse> GetCurrentUser()
    {
        var userId = User.FindFirstValue(
            JwtRegisteredClaimNames.Sub);
        var name = User.FindFirstValue(
            JwtRegisteredClaimNames.Name);
        var role = User.FindFirstValue("role");

        if (!long.TryParse(userId, out var parsedUserId) ||
            string.IsNullOrWhiteSpace(name) ||
            string.IsNullOrWhiteSpace(role))
        {
            return Unauthorized();
        }

        return Ok(new CurrentUserResponse(
            parsedUserId,
            name,
            role));
    }
}
