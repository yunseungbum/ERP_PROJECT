using BuddyErp.Api.DTOs.Schedules;
using BuddyErp.Api.Services.Schedules;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuddyErp.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class SchedulesController(
    IScheduleService scheduleService) : ControllerBase
{
    private const string ScheduleWriters =
        "President,Director,Coach,Treasurer,InventoryManager";

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ScheduleResponse>>> GetSchedules(
        CancellationToken cancellationToken)
    {
        var schedules = await scheduleService.GetSchedulesAsync(
            cancellationToken);

        return Ok(schedules);
    }

    [Authorize(Roles = ScheduleWriters)]
    [HttpPost]
    public async Task<ActionResult<ScheduleResponse>> CreateSchedule(
        ScheduleSaveRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var schedule = await scheduleService.CreateScheduleAsync(
                request,
                cancellationToken);

            return CreatedAtAction(
                nameof(GetSchedules),
                new { scheduleId = schedule.ScheduleId },
                schedule);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails { Detail = exception.Message });
        }
    }

    [Authorize(Roles = ScheduleWriters)]
    [HttpPut("{scheduleId:long}")]
    public async Task<ActionResult<ScheduleResponse>> UpdateSchedule(
        long scheduleId,
        ScheduleSaveRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var schedule = await scheduleService.UpdateScheduleAsync(
                scheduleId,
                request,
                cancellationToken);

            return schedule is null ? NotFound() : Ok(schedule);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails { Detail = exception.Message });
        }
    }

    [Authorize(Roles = ScheduleWriters)]
    [HttpDelete("{scheduleId:long}")]
    public async Task<IActionResult> DeleteSchedule(
        long scheduleId,
        CancellationToken cancellationToken)
    {
        var deleted = await scheduleService.DeleteScheduleAsync(
            scheduleId,
            cancellationToken);

        return deleted ? NoContent() : NotFound();
    }
}
