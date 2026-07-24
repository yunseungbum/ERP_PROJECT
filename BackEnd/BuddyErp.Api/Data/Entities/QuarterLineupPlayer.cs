namespace BuddyErp.Api.Data.Entities;

public sealed class QuarterLineupPlayer
{
    public long LineupPlayerId { get; set; }
    public long QuarterFormationId { get; set; }
    public long ParticipantId { get; set; }
    public required string SlotCode { get; set; }
    public int PositionOrder { get; set; }
    public QuarterFormation QuarterFormation { get; set; } = null!;
    public MatchParticipant Participant { get; set; } = null!;
}
