using System.ComponentModel.DataAnnotations;

namespace BuddyErp.Api.DTOs.Dues;

public sealed record DuesMatrixResponse(
    int Year,
    decimal MonthlyFee,
    decimal TotalExecutionAmount,
    decimal TotalPaidAmount,
    decimal TotalExpenseAmount,
    decimal BalanceAmount,
    decimal TotalUnpaidAmount,
    int UnpaidMemberCount,
    string SummaryNote,
    IReadOnlyList<DuesMemberResponse> Members);

public sealed record DuesMemberResponse(
    long MemberId,
    string MemberName,
    bool IsPaused,
    bool HasUniform,
    decimal ExecutionAmount,
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

public sealed record DuesExecutionUpdateRequest(
    [Range(0, 999999999999)] decimal Amount);

public sealed record DuesExecutionResponse(decimal Amount);

public sealed record DuesSummaryNoteUpdateRequest(
    [MaxLength(1000)] string? Content);

public sealed record DuesSummaryNoteResponse(string Content);
