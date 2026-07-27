namespace BuddyErp.Api.Data.Entities;

public sealed class Announcement
{
    public long AnnouncementId { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public required string AuthorName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
