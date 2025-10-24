using Microsoft.AspNetCore.Mvc;

namespace reactTestApp.Controllers.Api
{
    [ApiController]
    [Route("api/Connection")]
    public class ConnectionController : ControllerBase
    {
        private readonly ILogger<ConnectionController> _logger;

        public ConnectionController(ILogger<ConnectionController> logger)
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