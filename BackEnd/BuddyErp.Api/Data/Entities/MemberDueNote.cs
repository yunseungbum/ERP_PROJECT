namespace BuddyErp.Api.Data.Entities;

public sealed class MemberDueNote
{
    public long MemberDueNoteId { get; set; }
    public long MemberId { get; set; }
    public int DueYear { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Member Member { get; set; } = null!;
}
