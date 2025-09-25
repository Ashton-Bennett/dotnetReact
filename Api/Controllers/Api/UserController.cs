using Microsoft.AspNetCore.Mvc;
using Api.Models;
using Api.Services;

namespace Api.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;
        public UserController(IUserService service) => _service = service;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
           var users = await _service.GetAllAsync();

              return Ok(users);
        }

        [HttpGet("id")]
        public async Task<ActionResult<User>> GetById(int id)
        {
            var user = await _service.GetByIdAsync(id);
            return user != null ? Ok(user) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] User newUser)
        {
            var createdUser = await _service.Create(newUser);
            return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
        }
    }
}
