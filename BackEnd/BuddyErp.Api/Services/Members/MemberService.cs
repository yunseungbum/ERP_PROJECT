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
            .OrderBy(member => member.MemberName)
            .Select(member => new MemberResponse(
                member.MemberId,
                member.MemberName,
                member.PrimaryPosition,
                member.SecondaryPosition,
                member.PhoneNumber,
                member.BirthYear,
                member.Notes,
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
            Notes = request.Notes.Trim(),
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
        member.Notes = request.Notes.Trim();
        member.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(member);
    }

    public async Task<bool> DeactivateMemberAsync(
        long memberId,
        CancellationToken cancellationToken = default)
    {
        var member = await dbContext.Members.SingleOrDefaultAsync(
            member => member.MemberId == memberId && member.IsActive,
            cancellationToken);

        if (member is null)
        {
            return false;
        }

        member.IsActive = false;
        member.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

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

        if (request.Notes.Length > 1000)
        {
            throw new ArgumentException("비고는 1,000자 이하여야 합니다.");
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
            member.IsActive,
            member.CreatedAt,
            member.UpdatedAt);
    }
}
