using System.ComponentModel.DataAnnotations;

namespace BuddyErp.Api.DTOs.Dues;

public sealed record DuesMatrixResponse(
    int Year,
    decimal MonthlyFee,
    decimal TotalPaidAmount,
    decimal TotalUnpaidAmount,
    int UnpaidMemberCount,
    IReadOnlyList<DuesMemberResponse> Members);

public sealed record DuesMemberResponse(
    long MemberId,
    string MemberName,
    bool IsPaused,
    bool HasUniform,
    decimal PaidTotal,
    decimal UnpaidTotal,
    string Note,
    IReadOnlyList<DuesCellResponse> Dues);

public sealed record DuesCellResponse(
    int Month,
    DateTime DueDate,
    string Status);

public sealed record DuesUpdateRequest(
    [Required, RegularExpression("^(O|X|-)$")] string Status);

public sealed record DuesNoteUpdateRequest(
    [MaxLength(1000)] string? Content);

public sealed record DuesNoteResponse(string Content);
