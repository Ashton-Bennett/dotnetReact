using Api.Models.Data;
using Api.Models.DTOs;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Api.Enums;

namespace Api.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;
        public UserController(IUserService service) => _service = service;

        [HttpGet("GetAll")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
           var users = await _service.GetAllAsync();

              return Ok(users);
        }

        [HttpGet("GetRoles")]
        [Authorize(Roles = "Admin")]
        public ActionResult<IEnumerable<RoleDto>> GetRoles()
        {
            var roles = EnumExtensions.ToRoleDtoList();
            return Ok(roles);
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
            var result = await _service.CreateAsync(newUser);

            if (!result.Success)
                return BadRequest(new { message = result.ErrorMessage });

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, new { Message = "success"});
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
            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState);
            } 

            var success = await _service.UpdateAsync(id, newUser);
            if (!success) 
            {
                return NotFound();
            } 

            return NoContent();
        }


        [HttpPut("{id}/password")]
        [Authorize(Policy = "AdminOrSelf")]
        public async Task<IActionResult> UpdatePassword(int id, [FromBody] Dictionary<string, string> collection)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _service.UpdatePasswordAsync(id, collection["currentPassword"], collection["newPassword"]);

            if (!result.Success)
                return BadRequest(new { message = result.ErrorMessage });

            return Ok(result);
        }
    }
}
