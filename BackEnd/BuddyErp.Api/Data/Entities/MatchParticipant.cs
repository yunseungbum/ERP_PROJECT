namespace BuddyErp.Api.Data.Entities;

public sealed class MatchParticipant
{
    public long ParticipantId { get; set; }
    public long ScheduleId { get; set; }
    public long? MemberId { get; set; }
    public string? GuestName { get; set; }
    public bool IsGuest { get; set; }
    public DateTime CreatedAt { get; set; }
    public MatchSchedule Schedule { get; set; } = null!;
    public Member? Member { get; set; }
    public ICollection<QuarterLineupPlayer> LineupPlayers { get; set; } = [];
}
