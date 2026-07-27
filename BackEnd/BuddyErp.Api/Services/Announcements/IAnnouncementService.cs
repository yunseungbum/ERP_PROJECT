using BuddyErp.Api.DTOs.Announcements;

namespace BuddyErp.Api.Services.Announcements;

public interface IAnnouncementService
{
    Task<IReadOnlyList<AnnouncementResponse>> GetAnnouncementsAsync(
        CancellationToken cancellationToken = default);

    Task<AnnouncementResponse> CreateAnnouncementAsync(
        AnnouncementSaveRequest request,
        string authorName,
        CancellationToken cancellationToken = default);

    Task<AnnouncementResponse?> UpdateAnnouncementAsync(
        long announcementId,
        AnnouncementSaveRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAnnouncementAsync(
        long announcementId,
        CancellationToken cancellationToken = default);
}
