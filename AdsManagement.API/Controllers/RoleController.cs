using AdsManagement.App.DTOs.Role;
using AdsManagement.App.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;

namespace AdsManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _service;
        public RoleController(IRoleService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRole([FromRoute] Guid id, CancellationToken token = default)
        {
            var role = await _service.GetRoleAsync(id, token);
            return Ok(role);
        }
        [HttpGet("all")]
        public async Task<IActionResult> GetAllRoles([FromQuery] int page, [FromQuery] int pageSize, CancellationToken token = default)
        {
            var roles = await _service.GetAllRolesAsync(page, pageSize, token);
            return Ok(roles);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto roleDto, CancellationToken token = default)
        {
            var resultID = await _service.AddRoleAsync(roleDto, token);
            var role = await _service.GetRoleAsync(resultID, token);

            return CreatedAtAction(nameof(GetRole), new { id = resultID }, role);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole([FromRoute] Guid id, CancellationToken token = default)
        {
            await _service.DeleteRoleAsync(id, token);
            return NoContent();
        }
        [HttpPut]
        public async Task<ActionResult> UpdateRole([FromBody] UpdateRoleDto roleDto, CancellationToken token = default)
        {
            await _service.UpdateRoleAsync(roleDto, token);
            return NoContent();
        }
    }
}
