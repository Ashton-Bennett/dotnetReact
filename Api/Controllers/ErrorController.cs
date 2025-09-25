using Api.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/Error")]
public class ErrorController : ControllerBase
{
    [HttpGet("{statusCode}")]
    public IActionResult Index(int statusCode)
    {
        return statusCode switch
        {
            404 => NotFound(new { Message = "Resource not found" }),
            _ => Problem("An error occurred")
        };
    }
}
