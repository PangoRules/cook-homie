using Microsoft.AspNetCore.Mvc;

namespace CookHomie.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PollingController : ControllerBase
    {
        [HttpGet("data")]
        public IActionResult GetPollingData()
        {
            // Simulate some data that would come from a polling source
            var data = new
            {
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                itemsCount = new Random().Next(1, 100),
                lastPolled = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };

            return Ok(data);
        }
    }
}