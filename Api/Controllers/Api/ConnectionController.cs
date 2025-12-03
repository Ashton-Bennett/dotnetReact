using Microsoft.AspNetCore.Mvc;

namespace reactTestApp.Controllers.Api
{
    [ApiController]
    [Route("api/Connection")]
    public class ConnectionController : ControllerBase
    {

        public ConnectionController()
        {

        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { message = "Connected." });
        }
    }
}