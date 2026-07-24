namespace BuddyErp.Api.DTOs.Formations;

public sealed record FormationBoardResponse(
    long ScheduleId,
    string MatchTitle,
    DateTime StartsAt,
    IReadOnlyList<MatchParticipantResponse> Participants,
    IReadOnlyList<QuarterFormationResponse> Quarters);

public sealed record MatchParticipantResponse(
    long ParticipantId,
    long? MemberId,
    string ParticipantName,
    bool IsGuest,
    IReadOnlyList<bool> QuarterParticipation);

public sealed record QuarterFormationResponse(
    int QuarterNumber,
    string FormationCode,
    IReadOnlyList<LineupPlayerResponse> Players,
    DateTime? UpdatedAt);

public sealed record LineupPlayerResponse(
    long ParticipantId,
    string SlotCode,
    int PositionOrder);
