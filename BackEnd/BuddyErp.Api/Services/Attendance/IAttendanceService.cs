using BuddyErp.Api.DTOs.Attendance;

namespace BuddyErp.Api.Services.Attendance;

public interface IAttendanceService
{
    Task<AttendanceMatrixResponse> GetAttendanceMatrixAsync(
        CancellationToken cancellationToken = default);

    Task<AttendanceCellResponse?> UpdateAttendanceAsync(
        long scheduleId,
        long memberId,
        AttendanceUpdateRequest request,
        CancellationToken cancellationToken = default);
}
