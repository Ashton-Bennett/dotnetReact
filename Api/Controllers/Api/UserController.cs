using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;
        public UserController(IUserService service) => _service = service;

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
           var users = await _service.GetAllAsync();

              return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOrSelf")]
        public async Task<ActionResult<User>> GetById(int id)
        {
            var user = await _service.GetByIdAsync(id);
            return user != null ? Ok(user) : NotFound();
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] User newUser)
        {
            var createdUser = await _service.CreateAsync(newUser);
            return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOrSelf")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.Delete(id);

            if (!success)
            {
                return NotFound();  
            }

            return NoContent();
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOrSelf")]
        public async Task<IActionResult> Update(int id, [FromBody] User newUser)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var success = await _service.UpdateAsync(id, newUser);
            if (!success) return NotFound();

            return NoContent();
        }
    }
}
