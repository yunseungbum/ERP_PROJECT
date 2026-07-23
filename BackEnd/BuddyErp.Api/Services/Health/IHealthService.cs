using BuddyErp.Api.DTOs.Health;

namespace BuddyErp.Api.Services.Health;

public interface IHealthService
{
    HealthResponse GetStatus();
}
