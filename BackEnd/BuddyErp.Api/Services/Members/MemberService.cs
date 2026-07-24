using BuddyErp.Api.Data;
using BuddyErp.Api.Data.Entities;
using BuddyErp.Api.DTOs.Members;
using Microsoft.EntityFrameworkCore;

namespace BuddyErp.Api.Services.Members;

public sealed class MemberService(AppDbContext dbContext) : IMemberService
{
    public async Task<IReadOnlyList<MemberResponse>> GetMembersAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Members
            .AsNoTracking()
            .Where(member => member.IsActive)
            .OrderBy(member => member.MemberId)
            .Select(member => new MemberResponse(
                member.MemberId,
                member.MemberName,
                member.PrimaryPosition,
                member.SecondaryPosition,
                member.PhoneNumber,
                member.BirthYear,
                member.Notes,
                member.MemberStatus,
                member.IsActive,
                member.CreatedAt,
                member.UpdatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<MemberResponse?> GetMemberAsync(
        long memberId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Members
            .AsNoTracking()
            .Where(member =>
                member.MemberId == memberId && member.IsActive)
            .Select(member => new MemberResponse(
                member.MemberId,
                member.MemberName,
                member.PrimaryPosition,
                member.SecondaryPosition,
                member.PhoneNumber,
                member.BirthYear,
                member.Notes,
                member.MemberStatus,
                member.IsActive,
                member.CreatedAt,
                member.UpdatedAt))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<MemberResponse> CreateMemberAsync(
        MemberSaveRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateRequest(request);

        var now = DateTime.UtcNow;
        var member = new Member
        {
            MemberName = request.MemberName.Trim(),
            PrimaryPosition = request.PrimaryPosition,
            SecondaryPosition = request.SecondaryPosition,
            PhoneNumber = request.PhoneNumber.Trim(),
            BirthYear = request.BirthYear,
            Notes = (request.Notes ?? string.Empty).Trim(),
            MemberStatus = request.MemberStatus,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now,
        };

        dbContext.Members.Add(member);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(member);
    }

    public async Task<MemberResponse?> UpdateMemberAsync(
        long memberId,
        MemberSaveRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateRequest(request);

        var member = await dbContext.Members.SingleOrDefaultAsync(
            member => member.MemberId == memberId && member.IsActive,
            cancellationToken);

        if (member is null)
        {
            return null;
        }

        member.MemberName = request.MemberName.Trim();
        member.PrimaryPosition = request.PrimaryPosition;
        member.SecondaryPosition = request.SecondaryPosition;
        member.PhoneNumber = request.PhoneNumber.Trim();
        member.BirthYear = request.BirthYear;
        member.Notes = (request.Notes ?? string.Empty).Trim();
        member.MemberStatus = request.MemberStatus;
        member.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(member);
    }

    public async Task<bool> DeactivateMemberAsync(
        long memberId,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await dbContext.Database
            .BeginTransactionAsync(cancellationToken);

        var member = await dbContext.Members.SingleOrDefaultAsync(
            member => member.MemberId == memberId && member.IsActive,
            cancellationToken);

        if (member is null)
        {
            return false;
        }

        var futureMatchParticipants = await dbContext.MatchParticipants
            .Where(participant =>
                participant.MemberId == memberId &&
                participant.Schedule.StartsAt > DateTime.Now)
            .ToListAsync(cancellationToken);

        // match_participants를 삭제하면 DB의 외래키 설정에 따라
        // 미래 경기의 quarter_lineup_players도 함께 삭제됩니다.
        dbContext.MatchParticipants.RemoveRange(futureMatchParticipants);

        member.IsActive = false;
        member.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return true;
    }

    private static void ValidateRequest(MemberSaveRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.MemberName))
        {
            throw new ArgumentException("회원 이름은 필수입니다.");
        }

        if (!MemberPositionCodes.IsValid(request.PrimaryPosition))
        {
            throw new ArgumentException("올바른 1순위 포지션이 아닙니다.");
        }

        if (request.SecondaryPosition is not null &&
            !MemberPositionCodes.IsValid(request.SecondaryPosition))
        {
            throw new ArgumentException("올바른 2순위 포지션이 아닙니다.");
        }

        if (request.PrimaryPosition == request.SecondaryPosition)
        {
            throw new ArgumentException("1순위와 2순위 포지션은 달라야 합니다.");
        }

        if (string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            throw new ArgumentException("연락처는 필수입니다.");
        }

        if (request.BirthYear < 1900 ||
            request.BirthYear > DateTime.UtcNow.Year)
        {
            throw new ArgumentException("올바른 출생연도가 아닙니다.");
        }

        if (request.Notes?.Length > 1000)
        {
            throw new ArgumentException("비고는 1,000자 이하여야 합니다.");
        }

        if (!MemberStatusCodes.IsValid(request.MemberStatus))
        {
            throw new ArgumentException("올바른 회원 활동 상태가 아닙니다.");
        }
    }

    private static MemberResponse ToResponse(Member member)
    {
        return new MemberResponse(
            member.MemberId,
            member.MemberName,
            member.PrimaryPosition,
            member.SecondaryPosition,
            member.PhoneNumber,
            member.BirthYear,
            member.Notes,
            member.MemberStatus,
            member.IsActive,
            member.CreatedAt,
            member.UpdatedAt);
    }
}
