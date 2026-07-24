using BuddyErp.Api.DTOs.Formations;

namespace BuddyErp.Api.Services.Formations;

public interface IFormationService
{
    Task<FormationBoardResponse?> GetUpcomingBoardAsync(
        CancellationToken cancellationToken = default);

    Task<FormationBoardResponse?> AddMemberParticipantsAsync(
        long scheduleId,
        AddMemberParticipantsRequest request,
        CancellationToken cancellationToken = default);

    Task<FormationBoardResponse?> AddGuestParticipantAsync(
        long scheduleId,
        AddGuestParticipantRequest request,
        CancellationToken cancellationToken = default);

    Task<FormationBoardResponse?> RemoveParticipantAsync(
        long scheduleId,
        long participantId,
        CancellationToken cancellationToken = default);

    Task<FormationBoardResponse?> SaveQuarterFormationAsync(
        long scheduleId,
        int quarterNumber,
        SaveQuarterFormationRequest request,
        CancellationToken cancellationToken = default);
}
