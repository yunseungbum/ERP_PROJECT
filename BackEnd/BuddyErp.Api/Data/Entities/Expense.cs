namespace BuddyErp.Api.Data.Entities;

public sealed class Expense
{
    public long ExpenseId { get; set; }
    public long? ScheduleId { get; set; }
    public required string ExpenseItem { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string Notes { get; set; } = string.Empty;
    public required string PayerName { get; set; }
    public bool IsSettled { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public MatchSchedule? Schedule { get; set; }
}
