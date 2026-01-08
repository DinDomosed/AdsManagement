using AdsManagement.App.DTOs.User;
using AdsManagement.App.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;

namespace AdsManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;
        public UserController(IUserService service)
        {
            _service = service;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser ([FromRoute] Guid id, CancellationToken token = default)
        {
            var user = await _service.GetUserAsync(id, token);
            return Ok(user);
        }
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto userDto, CancellationToken token = default)
        {
            var newUserId = await _service.AddUserAsync(userDto, token);
            var user = await _service.GetUserAsync(newUserId, token);

            return CreatedAtAction(nameof(GetUser), new { id = newUserId }, user);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser([FromRoute] Guid id, [FromHeader(Name = "X-User-Id")] Guid requestUserId, CancellationToken token = default)
        {
            await _service.DeleteUserAsync(id, requestUserId, token);
            return NoContent();
        }
        [HttpPut]
        public async Task<ActionResult> UpdateUser([FromBody] UpdateUserDto userDto, [FromHeader(Name = "X-User-Id")] Guid requestUserId, CancellationToken token = default)
        {
            await _service.UpdateUserAsync(userDto, requestUserId, token);
            return NoContent();
        }
        [HttpGet("search")]
        public async Task<IActionResult> GetByFilter([FromQuery] UserFilterDto filterDto, CancellationToken token = default)
        {
            var paged = await _service.GetFilterUserAsync(filterDto, token);
            return Ok(paged);
        }

    }
}
