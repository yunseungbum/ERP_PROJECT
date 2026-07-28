using BuddyErp.Api.DTOs.Dues;
using BuddyErp.Api.Services.Dues;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuddyErp.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/dues")]
public sealed class DuesController(
    IMemberDueService memberDueService) : ControllerBase
{
    private const string DueWriters = "President,Treasurer";

    [HttpGet]
    public async Task<ActionResult<DuesMatrixResponse>> GetDues(
        [FromQuery] int year,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await memberDueService.GetDuesMatrixAsync(
                year,
                cancellationToken));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails { Detail = exception.Message });
        }
    }

    [Authorize(Roles = DueWriters)]
    [HttpPut("{year:int}/{month:int}/members/{memberId:long}")]
    public async Task<ActionResult<DuesCellResponse>> UpdateDue(
        int year,
        int month,
        long memberId,
        DuesUpdateRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var due = await memberDueService.UpdateDueAsync(
                memberId,
                year,
                month,
                request,
                cancellationToken);
            return due is null ? NotFound() : Ok(due);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails { Detail = exception.Message });
        }
    }

    [Authorize(Roles = DueWriters)]
    [HttpPut("{year:int}/members/{memberId:long}/note")]
    public async Task<ActionResult<DuesNoteResponse>> UpdateDueNote(
        int year,
        long memberId,
        DuesNoteUpdateRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var note = await memberDueService.UpdateDueNoteAsync(
                memberId,
                year,
                request,
                cancellationToken);
            return note is null ? NotFound() : Ok(note);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails { Detail = exception.Message });
        }
    }
}
