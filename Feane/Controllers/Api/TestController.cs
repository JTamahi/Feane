using Microsoft.AspNetCore.Mvc;

namespace Feane.Controllers.Api;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    [HttpGet("throw")]
    public IActionResult Throw()
        => throw new Exception("Test API exception");
}
