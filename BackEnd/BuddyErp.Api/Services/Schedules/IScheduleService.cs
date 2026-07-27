using BuddyErp.Api.DTOs.Schedules;

namespace BuddyErp.Api.Services.Schedules;

public interface IScheduleService
{
    Task<IReadOnlyList<ScheduleResponse>> GetSchedulesAsync(
        CancellationToken cancellationToken = default);

    Task<ScheduleResponse> CreateScheduleAsync(
        ScheduleSaveRequest request,
        CancellationToken cancellationToken = default);

    Task<ScheduleResponse?> UpdateScheduleAsync(
        long scheduleId,
        ScheduleSaveRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteScheduleAsync(
        long scheduleId,
        CancellationToken cancellationToken = default);
}
