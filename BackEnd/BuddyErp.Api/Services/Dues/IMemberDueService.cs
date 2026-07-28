using BuddyErp.Api.DTOs.Dues;

namespace BuddyErp.Api.Services.Dues;

public interface IMemberDueService
{
    Task<DuesMatrixResponse> GetDuesMatrixAsync(
        int year,
        CancellationToken cancellationToken = default);

    Task<DuesCellResponse?> UpdateDueAsync(
        long memberId,
        int year,
        int month,
        DuesUpdateRequest request,
        CancellationToken cancellationToken = default);

    Task<DuesNoteResponse?> UpdateDueNoteAsync(
        long memberId,
        int year,
        DuesNoteUpdateRequest request,
        CancellationToken cancellationToken = default);
}
