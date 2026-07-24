using BuddyErp.Api.DTOs.Formations;
using BuddyErp.Api.Services.Formations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuddyErp.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/formations")]
public sealed class FormationsController(
    IFormationService formationService) : ControllerBase
{
    private const string FormationWriters =
        "President,Director,Coach";

    [HttpGet("upcoming")]
    public async Task<ActionResult<FormationBoardResponse>> GetUpcomingBoard(
        CancellationToken cancellationToken)
    {
        var board = await formationService.GetUpcomingBoardAsync(
            cancellationToken);

        return board is null
            ? NotFound()
            : Ok(board);
    }

    [Authorize(Roles = FormationWriters)]
    [HttpPost("{scheduleId:long}/participants/members")]
    public async Task<ActionResult<FormationBoardResponse>> AddMembers(
        long scheduleId,
        AddMemberParticipantsRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var board = await formationService.AddMemberParticipantsAsync(
                scheduleId,
                request,
                cancellationToken);

            return board is null
                ? NotFound()
                : Ok(board);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails
            {
                Detail = exception.Message,
            });
        }
    }

    [Authorize(Roles = FormationWriters)]
    [HttpPost("{scheduleId:long}/participants/guests")]
    public async Task<ActionResult<FormationBoardResponse>> AddGuest(
        long scheduleId,
        AddGuestParticipantRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var board = await formationService.AddGuestParticipantAsync(
                scheduleId,
                request,
                cancellationToken);

            return board is null
                ? NotFound()
                : Ok(board);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails
            {
                Detail = exception.Message,
            });
        }
    }

    [Authorize(Roles = FormationWriters)]
    [HttpDelete("{scheduleId:long}/participants/{participantId:long}")]
    public async Task<ActionResult<FormationBoardResponse>> RemoveParticipant(
        long scheduleId,
        long participantId,
        CancellationToken cancellationToken)
    {
        var board = await formationService.RemoveParticipantAsync(
            scheduleId,
            participantId,
            cancellationToken);

        return board is null
            ? NotFound()
            : Ok(board);
    }

    [Authorize(Roles = FormationWriters)]
    [HttpPut("{scheduleId:long}/quarters/{quarterNumber:int}")]
    public async Task<ActionResult<FormationBoardResponse>> SaveQuarter(
        long scheduleId,
        int quarterNumber,
        SaveQuarterFormationRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var board = await formationService.SaveQuarterFormationAsync(
                scheduleId,
                quarterNumber,
                request,
                cancellationToken);

            return board is null
                ? NotFound()
                : Ok(board);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails
            {
                Detail = exception.Message,
            });
        }
    }
}
