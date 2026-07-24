namespace BuddyErp.Api.Data.Entities;

public sealed class QuarterFormation
{
    public long QuarterFormationId { get; set; }
    public long ScheduleId { get; set; }
    public int QuarterNumber { get; set; }
    public required string FormationCode { get; set; }
    public DateTime UpdatedAt { get; set; }
    public MatchSchedule Schedule { get; set; } = null!;
    public ICollection<QuarterLineupPlayer> LineupPlayers { get; set; } = [];
}
