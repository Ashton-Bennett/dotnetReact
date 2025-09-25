using Microsoft.AspNetCore.Mvc;

namespace reactTestApp.Controllers.Api
{
    [ApiController]
    [Route("api/Greetings")]
    public class GreetingController : ControllerBase
    {
        private readonly ILogger<GreetingController> _logger;

        public GreetingController(ILogger<GreetingController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation("----------> GET: Initial backend test message sent");
            return Ok(new { message = "Connected." });
        }
    }
}