namespace BuddyErp.Api.Data.Entities;

public sealed class Member
{
    public long MemberId { get; set; }
    public required string MemberName { get; set; }
    public required string PrimaryPosition { get; set; }
    public string? SecondaryPosition { get; set; }
    public required string PhoneNumber { get; set; }
    public int BirthYear { get; set; }
    public string Notes { get; set; } = string.Empty;
    public required string MemberStatus { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
