using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Api
{

    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService _service;

        public WeatherController(IWeatherService service) => _service = service;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WeatherForecast>>> GetAll()
        {
            var forecasts = await _service.GetAllAsync();
            return Ok(forecasts);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "RequireAdministratorRole")]
        public async Task<ActionResult<WeatherForecast>> GetById(int id)
        {
            var forecast = await _service.GetByIdAsync(id);
            if (forecast == null) return NotFound();
            return Ok(forecast);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] WeatherForecast forecast)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _service.CreateAsync(forecast);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] WeatherForecast forecast)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var success = await _service.UpdateAsync(id, forecast);
            if (!success) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound();

            return NoContent();
        }
    }
}
