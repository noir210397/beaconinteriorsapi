using beaconinteriorsapi.Controllers.Base;
using beaconinteriorsapi.DTOS;
using beaconinteriorsapi.Models;
using beaconinteriorsapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static beaconinteriorsapi.Exceptions.ExceptionHelpers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace beaconinteriorsapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController :MyBaseController
    {
        private readonly UserService _userService;
        protected override string ResourceName => "User";
        public UserController(UserService userService)
        {
            _userService = userService;
        }
        // GET: api/<UserController>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult>  GetUsers()
        {
            return Ok(await _userService.GetAllUsersAsync());
        }
        [HttpGet("role")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> GetUsersByRole(string role)
        {
             bool isValid=Enum.GetNames(typeof(UserRoleType)).Any(s=>s==role);
            if (!isValid) ThrowBadRequest($"no role of type {role}");
            return Ok(await _userService.GetUsersByRoleAsync(role));
        }

        // GET api/<UserController>/5
        [HttpGet("search")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> GetUserByEmail([FromQuery] string value)
        {
            if (string.IsNullOrEmpty(value)) return BadRequest("please provide a valid value to search with");
            return Ok(await _userService.GetUsersByEmailOrNameAsync(value));
        }
        [HttpGet("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> GetUserById(string id)
        {
            ValidateString(id);
            return Ok(await _userService.GetUserByIdAsync(id));
        }

        // POST api/<UserController>
        [HttpPut("{id}")]
        [Authorize(Roles ="User")]
        public async Task<IActionResult> UpdateUserInfo(string id,[FromBody] UserDTO user)
        {
            ValidateString(id);
            await _userService.UpdateUserInfoAsync(id, user.FirstName, user.LastName);
            return NoContent();
        }
       
        [HttpPut("superadmin/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> UpdateUserRoleToSuperAdmin(string id)
        {
            ValidateString(id);
            await _userService.MakeSuperAdminAsync(id);
            return NoContent();
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(string id)
        {
            ValidateString(id);
            await _userService.DeleteUserAsync(id);
            return NoContent();
        }
    }
}
