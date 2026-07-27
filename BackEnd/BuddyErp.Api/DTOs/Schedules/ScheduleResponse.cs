namespace BuddyErp.Api.DTOs.Schedules;

public sealed record ScheduleResponse(
    long ScheduleId,
    string VenueName,
    string OpponentName,
    DateTime StartsAt,
    decimal MatchFee,
    bool IsMatchFeePaid,
    string PayerName,
    string Notes,
    bool IsCompleted,
    string? OpponentContact);
