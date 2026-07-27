namespace BuddyErp.Api.Data.Entities;

public sealed class MatchAttendance
{
    public long AttendanceId { get; set; }
    public long ScheduleId { get; set; }
    public long MemberId { get; set; }
    public required string Status { get; set; }
    public DateTime UpdatedAt { get; set; }
    public MatchSchedule Schedule { get; set; } = null!;
    public Member Member { get; set; } = null!;
}
