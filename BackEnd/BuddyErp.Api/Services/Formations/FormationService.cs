using BuddyErp.Api.Data;
using BuddyErp.Api.Data.Entities;
using BuddyErp.Api.DTOs.Formations;
using Microsoft.EntityFrameworkCore;

namespace BuddyErp.Api.Services.Formations;

public sealed class FormationService(AppDbContext dbContext)
    : IFormationService
{
    public async Task<FormationBoardResponse?> GetUpcomingBoardAsync(
        CancellationToken cancellationToken = default)
    {
        var scheduleId = await dbContext.MatchSchedules
            .AsNoTracking()
            .Where(schedule => schedule.StartsAt >= DateTime.Now.Date)
            .OrderBy(schedule => schedule.StartsAt)
            .Select(schedule => (long?)schedule.ScheduleId)
            .FirstOrDefaultAsync(cancellationToken);

        return scheduleId is null
            ? null
            : await GetBoardAsync(scheduleId.Value, cancellationToken);
    }

    public async Task<FormationBoardResponse?> AddMemberParticipantsAsync(
        long scheduleId,
        AddMemberParticipantsRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.MemberIds.Count == 0)
        {
            throw new ArgumentException("추가할 회원을 선택해 주세요.");
        }

        var scheduleExists = await dbContext.MatchSchedules
            .AnyAsync(
                schedule => schedule.ScheduleId == scheduleId,
                cancellationToken);

        if (!scheduleExists)
        {
            return null;
        }

        var memberIds = request.MemberIds.Distinct().ToArray();
        var activeMemberIds = await dbContext.Members
            .AsNoTracking()
            .Where(member =>
                memberIds.Contains(member.MemberId) &&
                member.IsActive)
            .Select(member => member.MemberId)
            .ToListAsync(cancellationToken);

        if (activeMemberIds.Count != memberIds.Length)
        {
            throw new ArgumentException(
                "존재하지 않거나 활동 중이 아닌 회원이 포함되어 있습니다.");
        }

        var existingMemberIds = await dbContext.MatchParticipants
            .AsNoTracking()
            .Where(participant =>
                participant.ScheduleId == scheduleId &&
                participant.MemberId != null &&
                memberIds.Contains(participant.MemberId.Value))
            .Select(participant => participant.MemberId!.Value)
            .ToListAsync(cancellationToken);

        var existingMemberIdSet = existingMemberIds.ToHashSet();
        var now = DateTime.UtcNow;

        foreach (var memberId in memberIds.Where(
            memberId => !existingMemberIdSet.Contains(memberId)))
        {
            dbContext.MatchParticipants.Add(new MatchParticipant
            {
                ScheduleId = scheduleId,
                MemberId = memberId,
                GuestName = null,
                IsGuest = false,
                CreatedAt = now,
            });
        }

        var excludedAttendances = await dbContext.MatchAttendances
            .Where(attendance =>
                attendance.ScheduleId == scheduleId &&
                memberIds.Contains(attendance.MemberId) &&
                attendance.Status == "-")
            .ToListAsync(cancellationToken);

        dbContext.MatchAttendances.RemoveRange(excludedAttendances);
        await dbContext.SaveChangesAsync(cancellationToken);

        return await GetBoardAsync(scheduleId, cancellationToken);
    }

    public async Task<FormationBoardResponse?> AddGuestParticipantAsync(
        long scheduleId,
        AddGuestParticipantRequest request,
        CancellationToken cancellationToken = default)
    {
        var guestName = request.GuestName.Trim();

        if (string.IsNullOrWhiteSpace(guestName))
        {
            throw new ArgumentException("용병 이름을 입력해 주세요.");
        }

        if (guestName.Length > 50)
        {
            throw new ArgumentException("용병 이름은 50자 이하여야 합니다.");
        }

        var scheduleExists = await dbContext.MatchSchedules
            .AnyAsync(
                schedule => schedule.ScheduleId == scheduleId,
                cancellationToken);

        if (!scheduleExists)
        {
            return null;
        }

        var guestExists = await dbContext.MatchParticipants
            .AnyAsync(
                participant =>
                    participant.ScheduleId == scheduleId &&
                    participant.IsGuest &&
                    participant.GuestName == guestName,
                cancellationToken);

        if (!guestExists)
        {
            dbContext.MatchParticipants.Add(new MatchParticipant
            {
                ScheduleId = scheduleId,
                MemberId = null,
                GuestName = guestName,
                IsGuest = true,
                CreatedAt = DateTime.UtcNow,
            });

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return await GetBoardAsync(scheduleId, cancellationToken);
    }

    public async Task<FormationBoardResponse?> RemoveParticipantAsync(
        long scheduleId,
        long participantId,
        CancellationToken cancellationToken = default)
    {
        var participant = await dbContext.MatchParticipants
            .SingleOrDefaultAsync(
                item =>
                    item.ScheduleId == scheduleId &&
                    item.ParticipantId == participantId,
                cancellationToken);

        if (participant is null)
        {
            return null;
        }

        dbContext.MatchParticipants.Remove(participant);
        await dbContext.SaveChangesAsync(cancellationToken);

        return await GetBoardAsync(scheduleId, cancellationToken);
    }

    public async Task<FormationBoardResponse?> SaveQuarterFormationAsync(
        long scheduleId,
        int quarterNumber,
        SaveQuarterFormationRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateQuarterRequest(quarterNumber, request);

        var scheduleExists = await dbContext.MatchSchedules
            .AnyAsync(
                schedule => schedule.ScheduleId == scheduleId,
                cancellationToken);

        if (!scheduleExists)
        {
            return null;
        }

        var requestParticipantIds = request.Players
            .Select(player => player.ParticipantId)
            .Distinct()
            .ToArray();

        var validParticipantCount = await dbContext.MatchParticipants
            .AsNoTracking()
            .CountAsync(
                participant =>
                    participant.ScheduleId == scheduleId &&
                    requestParticipantIds.Contains(participant.ParticipantId),
                cancellationToken);

        if (validParticipantCount != requestParticipantIds.Length)
        {
            throw new ArgumentException(
                "해당 경기의 참여 인원이 아닌 선수가 포함되어 있습니다.");
        }

        await using var transaction = await dbContext.Database
            .BeginTransactionAsync(cancellationToken);

        var quarterFormation = await dbContext.QuarterFormations
            .Include(formation => formation.LineupPlayers)
            .SingleOrDefaultAsync(
                formation =>
                    formation.ScheduleId == scheduleId &&
                    formation.QuarterNumber == quarterNumber,
                cancellationToken);

        if (quarterFormation is null)
        {
            quarterFormation = new QuarterFormation
            {
                ScheduleId = scheduleId,
                QuarterNumber = quarterNumber,
                FormationCode = request.FormationCode,
                UpdatedAt = DateTime.UtcNow,
            };
            dbContext.QuarterFormations.Add(quarterFormation);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        else
        {
            quarterFormation.FormationCode = request.FormationCode;
            quarterFormation.UpdatedAt = DateTime.UtcNow;
            dbContext.QuarterLineupPlayers.RemoveRange(
                quarterFormation.LineupPlayers);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        dbContext.QuarterLineupPlayers.AddRange(
            request.Players.Select(player => new QuarterLineupPlayer
            {
                QuarterFormationId = quarterFormation.QuarterFormationId,
                ParticipantId = player.ParticipantId,
                SlotCode = player.SlotCode,
                PositionOrder = player.PositionOrder,
            }));

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return await GetBoardAsync(scheduleId, cancellationToken);
    }

    private async Task<FormationBoardResponse?> GetBoardAsync(
        long scheduleId,
        CancellationToken cancellationToken)
    {
        var schedule = await dbContext.MatchSchedules
            .AsNoTracking()
            .AsSplitQuery()
            .Include(item => item.Participants)
                .ThenInclude(participant => participant.Member)
            .Include(item => item.QuarterFormations)
                .ThenInclude(formation => formation.LineupPlayers)
            .SingleOrDefaultAsync(
                item => item.ScheduleId == scheduleId,
                cancellationToken);

        if (schedule is null)
        {
            return null;
        }

        var quarterResponses = Enumerable.Range(1, 4)
            .Select(quarterNumber =>
            {
                var formation = schedule.QuarterFormations
                    .SingleOrDefault(item =>
                        item.QuarterNumber == quarterNumber);

                return new QuarterFormationResponse(
                    quarterNumber,
                    formation?.FormationCode ?? "4-2-3-1",
                    formation?.LineupPlayers
                        .OrderBy(player => player.PositionOrder)
                        .Select(player => new LineupPlayerResponse(
                            player.ParticipantId,
                            player.SlotCode,
                            player.PositionOrder))
                        .ToList()
                        ?? [],
                    formation?.UpdatedAt);
            })
            .ToList();

        var participantResponses = schedule.Participants
            .OrderBy(participant =>
                participant.IsGuest
                    ? participant.GuestName
                    : participant.Member!.MemberName)
            .Select(participant => new MatchParticipantResponse(
                participant.ParticipantId,
                participant.MemberId,
                participant.IsGuest
                    ? participant.GuestName!
                    : participant.Member!.MemberName,
                participant.IsGuest,
                Enumerable.Range(1, 4)
                    .Select(quarterNumber =>
                        quarterResponses
                            .Single(quarter =>
                                quarter.QuarterNumber == quarterNumber)
                            .Players
                            .Any(player =>
                                player.ParticipantId ==
                                participant.ParticipantId))
                    .ToList()))
            .ToList();

        return new FormationBoardResponse(
            schedule.ScheduleId,
            $"{schedule.VenueName} {schedule.StartsAt:HH시} " +
                $"(vs {schedule.OpponentName})",
            schedule.StartsAt,
            participantResponses,
            quarterResponses);
    }

    private static void ValidateQuarterRequest(
        int quarterNumber,
        SaveQuarterFormationRequest request)
    {
        if (quarterNumber is < 1 or > 4)
        {
            throw new ArgumentException("쿼터는 1부터 4까지 가능합니다.");
        }

        if (!FormationCodes.IsValid(request.FormationCode))
        {
            throw new ArgumentException("올바른 포메이션 코드가 아닙니다.");
        }

        if (request.Players.Count > 11)
        {
            throw new ArgumentException("필드에는 최대 11명까지 배치할 수 있습니다.");
        }

        if (request.Players
            .GroupBy(player => player.ParticipantId)
            .Any(group => group.Count() > 1))
        {
            throw new ArgumentException(
                "같은 선수를 한 쿼터에 중복 배치할 수 없습니다.");
        }

        if (request.Players
            .GroupBy(player => player.SlotCode)
            .Any(group => group.Count() > 1))
        {
            throw new ArgumentException(
                "같은 위치에 두 명을 배치할 수 없습니다.");
        }

        if (request.Players.Any(player =>
            !FormationCodes.IsValidSlot(
                request.FormationCode,
                player.SlotCode)))
        {
            throw new ArgumentException(
                "선택한 포메이션에 없는 위치가 포함되어 있습니다.");
        }
    }
}
