using BuddyErp.Api.DTOs.Health;

namespace BuddyErp.Api.Services.Health;

public class HealthService : IHealthService
{
    public HealthResponse GetStatus()
    {
        return new HealthResponse("ok", DateTimeOffset.UtcNow);
    }
}
