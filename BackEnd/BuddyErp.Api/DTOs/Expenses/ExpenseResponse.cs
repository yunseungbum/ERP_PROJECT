namespace BuddyErp.Api.DTOs.Expenses;

using System.ComponentModel.DataAnnotations;

public sealed record ExpenseResponse(
    long ExpenseId,
    long? ScheduleId,
    string ExpenseItem,
    decimal Amount,
    DateTime PaymentDate,
    string Notes,
    string PayerName,
    bool IsSettled);

public sealed record ExpenseSummaryResponse(
    decimal TotalAmount,
    IReadOnlyDictionary<string, decimal> UnsettledAmounts,
    IReadOnlyList<ExpenseResponse> Expenses);

public sealed record ExpenseSettlementRequest(bool IsSettled);

public sealed record ExpenseSaveRequest(
    [Required, MaxLength(100)] string ExpenseItem,
    [Range(0, 999999999999)] decimal Amount,
    [Required] DateTime PaymentDate,
    [MaxLength(1000)] string? Notes,
    [Required, MaxLength(50)] string PayerName,
    bool IsSettled);
