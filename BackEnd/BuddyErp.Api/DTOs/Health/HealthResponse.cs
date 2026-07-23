namespace BuddyErp.Api.DTOs.Health;

public sealed record HealthResponse(
    string Status,
    DateTimeOffset CheckedAt);
