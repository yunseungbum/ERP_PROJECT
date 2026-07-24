namespace BuddyErp.Api.DTOs.Formations;

public sealed record AddMemberParticipantsRequest(
    IReadOnlyList<long> MemberIds);

public sealed record AddGuestParticipantRequest(
    string GuestName);

public sealed record SaveQuarterFormationRequest(
    string FormationCode,
    IReadOnlyList<SaveLineupPlayerRequest> Players);

public sealed record SaveLineupPlayerRequest(
    long ParticipantId,
    string SlotCode,
    int PositionOrder);
