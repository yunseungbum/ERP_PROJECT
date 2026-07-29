namespace BuddyErp.Api.Data.Entities;

public sealed class DuesYearSummary
{
    public long DuesYearSummaryId { get; set; }
    public int DueYear { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
