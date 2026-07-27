using BuddyErp.Api.DTOs.Announcements;
using BuddyErp.Api.Services.Announcements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuddyErp.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/announcements")]
public sealed class AnnouncementsController(
    IAnnouncementService announcementService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AnnouncementResponse>>>
        GetAnnouncements(CancellationToken cancellationToken)
    {
        return Ok(await announcementService.GetAnnouncementsAsync(
            cancellationToken));
    }

    [Authorize(Roles = "President")]
    [HttpPost]
    public async Task<ActionResult<AnnouncementResponse>> CreateAnnouncement(
        AnnouncementSaveRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await announcementService.CreateAnnouncementAsync(
                request,
                User.Identity?.Name ?? "회장",
                cancellationToken));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails { Detail = exception.Message });
        }
    }

    [Authorize(Roles = "President")]
    [HttpPut("{announcementId:long}")]
    public async Task<ActionResult<AnnouncementResponse>> UpdateAnnouncement(
        long announcementId,
        AnnouncementSaveRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var announcement =
                await announcementService.UpdateAnnouncementAsync(
                    announcementId,
                    request,
                    cancellationToken);
            return announcement is null ? NotFound() : Ok(announcement);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails { Detail = exception.Message });
        }
    }

    [Authorize(Roles = "President")]
    [HttpDelete("{announcementId:long}")]
    public async Task<IActionResult> DeleteAnnouncement(
        long announcementId,
        CancellationToken cancellationToken)
    {
        var deleted = await announcementService.DeleteAnnouncementAsync(
            announcementId,
            cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
