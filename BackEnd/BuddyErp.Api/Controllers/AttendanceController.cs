using BuddyErp.Api.DTOs.Attendance;
using BuddyErp.Api.Services.Attendance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuddyErp.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/attendance")]
public sealed class AttendanceController(
    IAttendanceService attendanceService) : ControllerBase
{
    private const string AttendanceWriters =
        "President,Director,Coach,Treasurer,InventoryManager";

    [HttpGet("matrix")]
    public async Task<ActionResult<AttendanceMatrixResponse>> GetMatrix(
        CancellationToken cancellationToken)
    {
        var matrix = await attendanceService.GetAttendanceMatrixAsync(
            cancellationToken);

        return Ok(matrix);
    }

    [Authorize(Roles = AttendanceWriters)]
    [HttpPut("schedules/{scheduleId:long}/members/{memberId:long}")]
    public async Task<ActionResult<AttendanceCellResponse>> UpdateAttendance(
        long scheduleId,
        long memberId,
        AttendanceUpdateRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var attendance = await attendanceService.UpdateAttendanceAsync(
                scheduleId,
                memberId,
                request,
                cancellationToken);

            return attendance is null ? NotFound() : Ok(attendance);
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
