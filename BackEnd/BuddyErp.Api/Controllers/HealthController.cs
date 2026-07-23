using BuddyErp.Api.DTOs.Health;
using BuddyErp.Api.Services.Health;
using Microsoft.AspNetCore.Mvc;

namespace BuddyErp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController(IHealthService healthService) : ControllerBase
{
    [HttpGet]
    public ActionResult<HealthResponse> Get()
    {
        var response = healthService.GetStatus();

        return Ok(response);
    }
}
