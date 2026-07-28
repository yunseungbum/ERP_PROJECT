using BuddyErp.Api.Data;
using BuddyErp.Api.Data.Entities;
using BuddyErp.Api.DTOs.Attendance;
using BuddyErp.Api.DTOs.Members;
using Microsoft.EntityFrameworkCore;

namespace BuddyErp.Api.Services.Attendance;

public sealed class AttendanceService(AppDbContext dbContext)
    : IAttendanceService
{
    public async Task<AttendanceMatrixResponse> GetAttendanceMatrixAsync(
        CancellationToken cancellationToken = default)
    {
        var schedules = await dbContext.MatchSchedules
            .AsNoTracking()
            .OrderBy(schedule => schedule.StartsAt)
            .Select(schedule => new AttendanceScheduleResponse(
                schedule.ScheduleId,
                schedule.StartsAt))
            .ToListAsync(cancellationToken);

        var members = await dbContext.Members
            .AsNoTracking()
            .Where(member => member.IsActive)
            .OrderBy(member => member.MemberId)
            .Select(member => new
            {
                member.MemberId,
                member.MemberName,
                member.MemberStatus,
            })
            .ToListAsync(cancellationToken);

        var participatingMemberKeys = await dbContext.MatchParticipants
            .AsNoTracking()
            .Where(participant =>
                !participant.IsGuest &&
                participant.MemberId != null)
            .Select(participant => new
            {
                participant.ScheduleId,
                MemberId = participant.MemberId!.Value,
            })
            .ToListAsync(cancellationToken);

        var participatingMemberSet = participatingMemberKeys
            .Select(item => (item.ScheduleId, item.MemberId))
            .ToHashSet();

        var manualAttendances = await dbContext.MatchAttendances
            .AsNoTracking()
            .ToDictionaryAsync(
                attendance => (
                    attendance.ScheduleId,
                    attendance.MemberId),
                attendance => attendance.Status,
                cancellationToken);

        var memberResponses = members.Select(member =>
        {
            var isPaused =
                member.MemberStatus == MemberStatusCodes.Paused;

            var attendances = schedules
                .Select(schedule => new AttendanceCellResponse(
                    schedule.ScheduleId,
                    isPaused
                        ? manualAttendances.TryGetValue((
                            schedule.ScheduleId,
                            member.MemberId),
                            out var pausedManualStatus)
                            ? pausedManualStatus
                            : "-"
                        : manualAttendances.TryGetValue((
                            schedule.ScheduleId,
                            member.MemberId),
                            out var manualStatus)
                            ? manualStatus
                        : participatingMemberSet.Contains((
                            schedule.ScheduleId,
                            member.MemberId))
                            ? "O"
                            : "-"))
                .ToList();

            var applicableAttendances = isPaused
                ? []
                : attendances
                    .Where(attendance => attendance.Status != "-")
                    .ToList();
            var attendanceRate = applicableAttendances.Count == 0
                ? (decimal?)null
                : Math.Round(
                    applicableAttendances.Count(attendance =>
                        attendance.Status == "O") *
                    100m /
                    applicableAttendances.Count,
                    1);

            return new AttendanceMemberResponse(
                member.MemberId,
                member.MemberName,
                isPaused,
                attendanceRate,
                attendances);
        }).ToList();

        return new AttendanceMatrixResponse(
            schedules,
            memberResponses);
    }

    public async Task<AttendanceCellResponse?> UpdateAttendanceAsync(
        long scheduleId,
        long memberId,
        AttendanceUpdateRequest request,
        CancellationToken cancellationToken = default)
    {
        var status = request.Status.Trim().ToUpperInvariant();

        if (status is not ("O" or "X" or "-"))
        {
            throw new ArgumentException("참석 상태는 O 또는 X만 가능합니다.");
        }

        var scheduleExists = await dbContext.MatchSchedules
            .AnyAsync(
                schedule => schedule.ScheduleId == scheduleId,
                cancellationToken);
        var member = await dbContext.Members
            .SingleOrDefaultAsync(
                member =>
                    member.MemberId == memberId &&
                    member.IsActive,
                cancellationToken);

        if (!scheduleExists || member is null)
        {
            return null;
        }

        var attendance = await dbContext.MatchAttendances
            .SingleOrDefaultAsync(
                attendance =>
                    attendance.ScheduleId == scheduleId &&
                    attendance.MemberId == memberId,
                cancellationToken);

        if (attendance is null)
        {
            attendance = new MatchAttendance
            {
                ScheduleId = scheduleId,
                MemberId = memberId,
                Status = status,
                UpdatedAt = DateTime.UtcNow,
            };
            dbContext.MatchAttendances.Add(attendance);
        }
        else
        {
            attendance.Status = status;
            attendance.UpdatedAt = DateTime.UtcNow;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new AttendanceCellResponse(scheduleId, status);
    }
}
