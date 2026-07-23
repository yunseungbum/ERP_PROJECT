namespace BuddyErp.Api.Data.Entities;

public sealed class User
{
    public long UserId { get; set; }
    public long? MemberId { get; set; }
    public required string LoginId { get; set; }
    public required string PasswordHash { get; set; }
    public required string DisplayName { get; set; }
    public required string RoleCode { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Member? Member { get; set; }
}
