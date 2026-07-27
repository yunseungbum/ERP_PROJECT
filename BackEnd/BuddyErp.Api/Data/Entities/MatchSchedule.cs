namespace BuddyErp.Api.Data.Entities;

public sealed class MatchSchedule
{
    public long ScheduleId { get; set; }
    public required string VenueName { get; set; }
    public required string OpponentName { get; set; }
    public DateTime StartsAt { get; set; }
    public decimal MatchFee { get; set; }
    public bool IsMatchFeePaid { get; set; }
    public required string PayerName { get; set; }
    public string Notes { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public string? OpponentContact { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<MatchParticipant> Participants { get; set; } = [];
    public ICollection<QuarterFormation> QuarterFormations { get; set; } = [];
    public Expense? Expense { get; set; }
}
