namespace BuddyErp.Api.DTOs.Attendance;

public sealed record AttendanceMatrixResponse(
    IReadOnlyList<AttendanceScheduleResponse> Schedules,
    IReadOnlyList<AttendanceMemberResponse> Members);

public sealed record AttendanceScheduleResponse(
    long ScheduleId,
    DateTime StartsAt);

public sealed record AttendanceMemberResponse(
    long MemberId,
    string MemberName,
    bool IsPaused,
    decimal? AttendanceRate,
    IReadOnlyList<AttendanceCellResponse> Attendances);

public sealed record AttendanceCellResponse(
    long ScheduleId,
    string Status);
