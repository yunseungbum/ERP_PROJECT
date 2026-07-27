using System.ComponentModel.DataAnnotations;

namespace BuddyErp.Api.DTOs.Schedules;

public sealed record ScheduleSaveRequest(
    [Required] DateTime StartsAt,
    [Required, MaxLength(100)] string OpponentName,
    [Required, MaxLength(100)] string VenueName,
    [Range(0, 999999999999)] decimal MatchFee,
    bool IsMatchFeePaid,
    [Required, MaxLength(50)] string PayerName,
    [MaxLength(1000)] string? Notes,
    bool IsCompleted,
    [MaxLength(30)] string? OpponentContact);
