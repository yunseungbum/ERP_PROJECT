namespace BuddyErp.Api.Data.Entities;

public sealed class MemberDue
{
    public long MemberDueId { get; set; }
    public long MemberId { get; set; }
    public int DueYear { get; set; }
    public int DueMonth { get; set; }
    public decimal Amount { get; set; }
    public required string PaymentStatus { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Member Member { get; set; } = null!;
}
