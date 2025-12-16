using Api.Models.Data;
using Api.Models.DTOs;
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

        [HttpGet("GetAll")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetAll()
        {
            ServiceResult<IEnumerable<User>> response = await _service.GetAllAsync();
            if (response.Success)
            {
                var returnResults = new ApiSuccessResponse<IEnumerable<User>>
                {
                    Data = response.Data ?? Enumerable.Empty<User>()
                };
                return Ok(returnResults);
            }
            else
            {
                var returnResults = new ApiErrorResponse
                {
                    Message = response.ErrorMessage ?? "An error occurred while retrieving users."
                };

                return StatusCode(500, returnResults);
            }
        }


        [HttpGet("GetRoles")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetRolesAsync()
        {
            ServiceResult<IEnumerable<RoleDto>> response = await _service.GetRolesAsync();
            if (response.Success)
            {
                var returnResults = new ApiSuccessResponse<IEnumerable<RoleDto>>
                {
                    Data = response.Data ?? Enumerable.Empty<RoleDto>()
                };

                return Ok(returnResults);
            }
            else
            {
                var returnResults = new ApiErrorResponse
                {
                    Message = response.ErrorMessage ?? "An error occurred while retrieving user roles."
                };

                return StatusCode(500, returnResults);
            }
        }


        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOrSelf")]
        [ProducesResponseType(typeof(ApiSuccessResponse<User>), StatusCodes.Status200OK)]  // Inlcude this once to get the types to swagger
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]  //Includes this once to get the types to swagger
        public async Task<ActionResult<User>> GetById(int id)
        {
            ServiceResult<User> response = await _service.GetByIdAsync(id);
            if (response.Success)
            {

                var returnResults = new ApiSuccessResponse<User>
                {
                    Data = response.Data
                };

                return Ok(returnResults);

            }
            else
            {
                var returnResults = new ApiErrorResponse
                {
                    Message = response.ErrorMessage ?? "User not found."
                };
                return NotFound(returnResults);
            }
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] User newUser)
        {
            ServiceResult<User> response = await _service.CreateAsync(newUser);
            if (response.Success)
            {

                var returnResults = new ApiSuccessResponse<User>
                {
                    Data = response.Data
                };

                return CreatedAtAction(nameof(GetById), new { id = returnResults.Data!.Id }, returnResults);
            }
            else
            {
                var returnResults = new ApiErrorResponse
                {
                    Message = response.ErrorMessage ?? "Unable to create user."
                };
                return BadRequest(returnResults);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOrSelf")]
        public async Task<IActionResult> Delete(int id)
        {
            ServiceResult<bool> response = await _service.Delete(id);

            if (response.Success)
            {
                return NoContent();
            }

            return NotFound(new ApiErrorResponse
            {
                Message = response.ErrorMessage ?? "Unable to delete user."
            });
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOrSelf")]
        public async Task<IActionResult> Update(int id, [FromBody] User newUser)
        {
            ServiceResult<bool> response = await _service.UpdateAsync(id, newUser);

            if (response.Success)
            {
                return NoContent();
            }

            return NotFound(new ApiErrorResponse
            {
                Message = response.ErrorMessage ?? "Unable to update user."
            });
        }


        [HttpPut("{id}/password")]
        [Authorize(Policy = "AdminOrSelf")]
        public async Task<IActionResult> UpdatePassword(int id, [FromBody] Dictionary<string, string> collection)
        {

            ServiceResult<object> result = await _service.UpdatePasswordAsync(id, collection["currentPassword"], collection["newPassword"]);

            if (!result.Success)
            {
                var returnResults = new ApiErrorResponse
                {
                    Message = result.ErrorMessage ?? "Unable to update password."
                };

                return BadRequest(returnResults);
            }
                

            return Ok(result);
        }
    }
}
