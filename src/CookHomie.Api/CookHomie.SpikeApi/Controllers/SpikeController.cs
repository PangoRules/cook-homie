using Microsoft.AspNetCore.Mvc;

namespace CookHomie.SpikeApi.Controllers;

[ApiController]
[Route("spike")]
public class SpikeController : ControllerBase
{
    [HttpGet("hello")]
    public IActionResult Hello()
    {
        return Ok(new
        {
            message = "Hello from C#",
            timestamp = DateTimeOffset.UtcNow
        });
    }
}
