using BuddyErp.Api.Data;
using BuddyErp.Api.Data.Entities;
using BuddyErp.Api.DTOs.Announcements;
using Microsoft.EntityFrameworkCore;

namespace BuddyErp.Api.Services.Announcements;

public sealed class AnnouncementService(
    AppDbContext dbContext) : IAnnouncementService
{
    private const int MaximumAnnouncementCount = 3;

    public async Task<IReadOnlyList<AnnouncementResponse>>
        GetAnnouncementsAsync(
            CancellationToken cancellationToken = default)
    {
        return await dbContext.Announcements
            .AsNoTracking()
            .OrderByDescending(item => item.CreatedAt)
            .ThenByDescending(item => item.AnnouncementId)
            .Take(MaximumAnnouncementCount)
            .Select(item => new AnnouncementResponse(
                item.AnnouncementId,
                item.Title,
                item.Content,
                item.AuthorName,
                item.CreatedAt,
                item.UpdatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<AnnouncementResponse> CreateAnnouncementAsync(
        AnnouncementSaveRequest request,
        string authorName,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var announcement = new Announcement
        {
            Title = ValidateText(request.Title, "공지 제목"),
            Content = ValidateText(request.Content, "공지 내용"),
            AuthorName = ValidateText(authorName, "작성자"),
            CreatedAt = now,
            UpdatedAt = now,
        };

        dbContext.Announcements.Add(announcement);
        await dbContext.SaveChangesAsync(cancellationToken);

        var oldAnnouncements = await dbContext.Announcements
            .OrderByDescending(item => item.CreatedAt)
            .ThenByDescending(item => item.AnnouncementId)
            .Skip(MaximumAnnouncementCount)
            .ToListAsync(cancellationToken);

        if (oldAnnouncements.Count > 0)
        {
            dbContext.Announcements.RemoveRange(oldAnnouncements);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return ToResponse(announcement);
    }

    public async Task<AnnouncementResponse?> UpdateAnnouncementAsync(
        long announcementId,
        AnnouncementSaveRequest request,
        CancellationToken cancellationToken = default)
    {
        var announcement = await dbContext.Announcements.SingleOrDefaultAsync(
            item => item.AnnouncementId == announcementId,
            cancellationToken);

        if (announcement is null)
        {
            return null;
        }

        announcement.Title = ValidateText(request.Title, "공지 제목");
        announcement.Content = ValidateText(request.Content, "공지 내용");
        announcement.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToResponse(announcement);
    }

    public async Task<bool> DeleteAnnouncementAsync(
        long announcementId,
        CancellationToken cancellationToken = default)
    {
        var announcement = await dbContext.Announcements.SingleOrDefaultAsync(
            item => item.AnnouncementId == announcementId,
            cancellationToken);

        if (announcement is null)
        {
            return false;
        }

        dbContext.Announcements.Remove(announcement);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static AnnouncementResponse ToResponse(Announcement item)
    {
        return new AnnouncementResponse(
            item.AnnouncementId,
            item.Title,
            item.Content,
            item.AuthorName,
            item.CreatedAt,
            item.UpdatedAt);
    }

    private static string ValidateText(string value, string fieldName)
    {
        var trimmedValue = value.Trim();
        if (trimmedValue.Length == 0)
        {
            throw new ArgumentException($"{fieldName}을 입력해 주세요.");
        }

        return trimmedValue;
    }
}
