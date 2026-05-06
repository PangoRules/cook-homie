using Microsoft.AspNetCore.Mvc;

namespace CookHomie.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController() : ControllerBase
{
    [HttpGet]
    public IActionResult Test()
    {
        return Ok();
    }
}
