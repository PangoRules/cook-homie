using Microsoft.AspNetCore.Mvc;

namespace CookHomie.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HelloController : ControllerBase
{
    [HttpGet]
    public ActionResult<HelloResponse> Get()
    {
        return Ok(new HelloResponse
        {
            Message = "Hello from C#"
        });
    }

    public sealed class HelloResponse
    {
        public string Message { get; set; } = string.Empty;
    }
}