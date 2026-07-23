using BuddyErp.Api.DTOs.Members;

namespace BuddyErp.Api.Services.Members;

public interface IMemberService
{
    Task<IReadOnlyList<MemberResponse>> GetMembersAsync(
        CancellationToken cancellationToken = default);

    Task<MemberResponse?> GetMemberAsync(
        long memberId,
        CancellationToken cancellationToken = default);

    Task<MemberResponse> CreateMemberAsync(
        MemberSaveRequest request,
        CancellationToken cancellationToken = default);

    Task<MemberResponse?> UpdateMemberAsync(
        long memberId,
        MemberSaveRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeactivateMemberAsync(
        long memberId,
        CancellationToken cancellationToken = default);
}
