using System.ComponentModel.DataAnnotations;

namespace BuddyErp.Api.DTOs.Announcements;

public sealed record AnnouncementResponse(
    long AnnouncementId,
    string Title,
    string Content,
    string AuthorName,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public sealed record AnnouncementSaveRequest(
    [Required, MaxLength(100)] string Title,
    [Required, MaxLength(1000)] string Content);
